using System;
using System.Collections.Generic;
using ConsoleSystem;
using Data;
using Entity;
using GamePlaySystem.FactionSystem;
using GamePlaySystem.SkillSystem;
using MyEventSystem;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace GamePlaySystem
{
    public class Character : BaseEntity, IComparable<Character>
    {
        // TODO 改到Property中
        public string CharacterName => property.characterName;
        public int SellPrice => property.sellPrice;
        
        [Header("角色属性")]
        [SerializeField] private CharacterProperty property;// 角色属性
        [SerializeField] private List<SkillSlot> skills;// 技能
        
        public readonly BindableProperty<FactionType> Faction = new();
        public CharacterProperty Property => property;
        public List<SkillSlot> Skills => skills;
        
        // BUFF区
        // private List<Buff> buffs = new();
        // public void AddBuff(Buff buff) => buffs.Add(buff);
        // public void RemoveBuffByType(BuffType buffType) 
        // public void RemoveBuffByDurationType(BuffDurationType buffDurationType)
        // public void RemoveBuff(Buff buff)
        
        public bool IsDead { get; private set; }
        public bool IsOnTurn { get; private set; }          // 是否正在行动
        public bool IsReadyToEndTurn{ get; private set; }   // 是否准备结束回合
        
        // public event Action OnStartTurnEvent;// 单位回合开始时只触发一次，用于通知监听者(例如Buff)
        // public event Action OnEndTurnEvent;  // 单位回合结束时只触发一次，用于通知监听者(例如Buff)
        public event Action<DamageType, int> OnTakeDamage; // int 为伤害值，用于通知监听，通常为Buff效果
        
        public event Action ReadyToEndEvent;  // 单位点击结束回合按钮时触发，用于通知监听
        public event Action CancelReadyToEndEvent; // 单位取消结束回合按钮时触发，用于通知监听
        public event Action<Character> OnDeathEvent; // 单位死亡时触发，用于通知监听，通常为Buff，瓦片。
        // 注意：event 的触发只能由Character自身进行，不允许外部触发
        public Action<SkillSlot> OnSkillChosenEnter; // 选择技能时触发，用于通知监听者(例如UI)，角色行为
        public Action OnSkillChosenExit; // 选择技能时触发，用于通知监听者(例如UI)，角色行为
        
        public void Initialize(Guid entityId, Vector3 position, FactionType factionType)
        {
            base.Initialize(entityId, position);
            // 深拷贝属性
            property = property.DeepCopy();
            property.Initialize();
            Faction.Value = factionType;
            // 在初始化完成后注册自身
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnCharacterCreated, this);
        }

        /// <summary>
        /// 角色进入回合时的操作
        /// </summary>
        public void StartTurn()
        {
            Property.AP.Value = Mathf.Min(Property.AP.Value + 2, CharacterProperty.AP_MAX);
            Property.RWR.Value = Property.WR_MAX.Value;
            CoolDownSkills();
            
            IsOnTurn = true;
            IsReadyToEndTurn = false;
        }

        /// 添加一个waitForEnd状态，用于等待玩家点击结束回合按钮
        /// <summary>
        /// 切换 玩家等待结束状态，由玩家点击结束回合按钮触发
        /// </summary>
        public void SwitchEndTurnReadyState()
        {
            // TODO 等待同批角色 行动结束
            IsReadyToEndTurn = !IsReadyToEndTurn;
            MyConsole.Print($"[准备结束状态切换] {CharacterName} {IsReadyToEndTurn}", MessageColor.Yellow);
            if (IsReadyToEndTurn) ReadyToEndEvent?.Invoke(); 
            else CancelReadyToEndEvent?.Invoke();
        }
        
        public void EndTurn()
        {
            IsOnTurn = false;
            IsReadyToEndTurn = true;
        }

        [ContextMenu("减少所有技能CD一回合")]
        private void CoolDownSkills()
        {
            foreach (var skillSlot in Skills)
            {
                skillSlot.CoolDownForOneRound();
            }
        }

        private void DeadAction()
        {
            IsDead = true;
            MyConsole.Print($"[角色死亡] {CharacterName}", MessageColor.Red);
            OnDeathEvent?.Invoke(this);
            // gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 需要伤害类型，以及计算后的伤害值
        /// </summary>
        /// <returns>角色是否因此死亡</returns>
        public bool TakeDamage(int damage, DamageType damageType)
        {
            OnTakeDamage?.Invoke(damageType, damage);
            // TODO 伤害计算,数值最好缓存一下，不会修改时会频繁触发监听事件
            // TODO 死后向战斗管理器发送死亡消息，由其决定对象的销毁
            var curHp = Property.HP.Value;

            switch (damageType)
            {
                case DamageType.Physical:
                    var def = Property.DEF.Value;
                    ApplyDamageOnDefence(ref damage, ref def);
                    Property.DEF.Value = def;
                    break;
                case DamageType.Magic:
                    var mdef = Property.MDEF.Value;
                    ApplyDamageOnDefence(ref damage, ref mdef);
                    Property.MDEF.Value = mdef;
                    break;
                case DamageType.True:
                    break;
            }
            curHp -= damage;
            curHp = Mathf.Clamp((int)curHp, (int)0, (int)Property.HP_MAX.Value);
            Property.HP.Value = curHp;

            if (curHp > 0) return false;
            DeadAction();
            return true;
        }
        
        /// <summary>
        /// 传入伤害值以及防御值，计算后修改伤害值和防御值
        /// </summary>
        private void ApplyDamageOnDefence(ref int damage, ref int defence){
            if (defence >= damage){
                defence -= damage;
                damage = 0;
            }else{
                damage -= defence;
                defence = 0;
            }
        }
        
        /// <summary>
        /// 角色排序规则，这里只是简单的按照敏捷值排序
        /// </summary>
        public int CompareTo(Character other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            // 敏捷值高的优先行动
            var order = other.Property.DEX.Value.CompareTo(Property.DEX.Value);
            if (order != 0) return order;
            // // 其次阵营靠前的优先行动
            // order = Faction.Value.CompareTo(other.Faction.Value);
            // if (order != 0) return order;
            // 最后随机
            return Random.Range(0, 2) == 0 ? -1 : 1;
        }
    }
}