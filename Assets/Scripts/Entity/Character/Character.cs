using System;
using System.Collections.Generic;
using BattleSystem.Entity.Character;
using BattleSystem.FactionSystem;
using BattleSystem.SkillSystem;
using ConsoleSystem;
using MyEventSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using Random = UnityEngine.Random;

namespace Entity.Character
{
    public enum DamageType
    {
        Physical,
        Magic,
        True
    }
    
    [Serializable] public class SkillSlot
    {
        [FormerlySerializedAs("Skill")] 
        public BaseSkill skill;
        public int remainCoolDown;

        public void CoolDown() => remainCoolDown = Mathf.Max(0, remainCoolDown - 1);
        public Action OnSkillUsed;
    }
    
    /// <summary>
    /// 角色被击倒后三回合后死亡
    /// </summary>
    public class Character : BaseEntity, IComparable<Character>//, IBurnable
    {
        [Header("角色属性")]
        public string characterName;
        public int sellPrice;
        [SerializeField] private CharacterProperty settingProperty;// 角色属性
        [SerializeField] private List<SkillSlot> skills;// 技能

        #region 属性区

        public readonly BindableProperty<FactionType> Faction = new();
        [HideInInspector] public CharacterProperty property;

        #endregion
        public List<SkillSlot> Skills => skills;
        // BUFF区
        // private List<Buff> buffs = new();
        // public void AddBuff(Buff buff) => buffs.Add(buff);
        // public void RemoveBuffByType(BuffType buffType) 
        // public void RemoveBuffByDurationType(BuffDurationType buffDurationType)
        // public void RemoveBuff(Buff buff)
        #region 角色驱动数据

        public bool IsDead { get; private set; }
        public bool IsOnTurn { get; private set; }          // 是否正在行动
        public bool IsReadyToEndTurn{ get; private set; }   // 是否准备结束回合
        
        #endregion

        #region 角色事件

        public event Action OnStartTurnEvent; // 单位回合开始时只触发一次，用于通知监听者(例如Buff)
        // public event Action OnEndTurnEvent;// 单位回合结束时只触发一次，用于通知监听者(例如Buff)
        public event Action ReadyToEndEvent;  // 单位点击结束回合按钮时触发，用于通知监听
        public event Action CancelReadyToEndEvent; // 单位取消结束回合按钮时触发，用于通知监听
        public event Action<Character> OnDeathEvent; // 单位死亡时触发，用于通知监听，通常为Buff，瓦片。
        // public event Action<DamageType, int> OnTakeDamage; // int 为伤害值，用于通知监听，通常为Buff效果
        
        #endregion

        
        public void Initialize(Guid entityID, Vector2 position, FactionType factionType)
        {
            base.Initialize(entityID, position);
            // 深拷贝属性
            property = settingProperty.DeepCopy();
            property.Initialize();
            Faction.Value = factionType;
            
            // 在初始化完成后注册自身
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnCharacterCreated, this);
            // CharacterManager.Instance.RegisterCharacter(this);
        }

        public void StartTurn()
        {
            // TODO 眩晕的时候要不要恢复？
            // 回合开始时的逻辑
            property.AP.Value = Mathf.Min(property.AP.Value + 2, CharacterProperty.AP_MAX);
            property.RWR.Value = property.WR_MAX.Value;
            foreach (var skillSlot in Skills) {
                skillSlot.CoolDown();
            }
            
            IsOnTurn = true;
            IsReadyToEndTurn = false;
            OnStartTurnEvent?.Invoke();
            
            // TODO 根据阵营不同，控制方式不同
            
            // 敌人行动时，根据AI选择技能或移动
            if (Faction.Value == FactionType.Enemy){
                // TODO 向AI处理队列添加自身，让其决定技能释放或移动
                MyConsole.Print($"[敌人行动] {characterName}", MessageColor.Yellow);
                SwitchEndTurnReadyState(); // 模拟点击了一次结束回合按钮
            }
        }
        
        public void EndTurn()
        {
            // TODO Do Something when character is inactive
            IsOnTurn = false;
            IsReadyToEndTurn = true;    // TODO 技能强制控制眩晕是否导致此
        }

        private void Dead()
        {
            IsDead = true;
            OnDeathEvent?.Invoke(this);
            // TODO 需要腾出位置来，通知瓦片清除自身。
            gameObject.SetActive(false);
            
            if (Faction.Value == FactionType.Player) {
                // TODO 玩家角色死亡，游戏结束
                MyConsole.Print($"[玩家角色死亡] {characterName}", MessageColor.Red);
            }
        }
        
        /// <summary>
        /// 需要伤害类型，以及计算后的伤害值
        /// </summary>
        /// <returns>角色是否因此死亡</returns>
        public bool TakeDamage(int damage, DamageType damageType)
        {
            // TODO 伤害计算,数值最好缓存一下，不会修改时会频繁触发监听事件
            // TODO 死后向战斗管理器发送死亡消息，由其决定对象的销毁
            var curHp = property.HP.Value;

            switch (damageType)
            {
                case DamageType.Physical:
                    var def = property.DEF.Value;
                    ApplyDamageOnDefence(ref damage, ref def);
                    property.DEF.Value = def;
                    break;
                case DamageType.Magic:
                    var mdef = property.MDEF.Value;
                    ApplyDamageOnDefence(ref damage, ref mdef);
                    property.MDEF.Value = mdef;
                    break;
                case DamageType.True:
                    break;
            }
            
            curHp -= damage;
            curHp = Mathf.Clamp(curHp, 0, property.HP_MAX.Value);
            property.HP.Value = curHp;
            
            // TODO 血量为0不会立马死亡
            if (curHp <= 0)
            {
                Dead();
                return true;
            }
            return false;
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

        /// 添加一个waitForEnd状态，用于等待玩家点击结束回合按钮
        /// <summary>
        /// 切换 玩家等待结束状态，由玩家点击结束回合按钮触发
        /// </summary>
        public void SwitchEndTurnReadyState()
        {
            // TODO 等待同批角色 行动结束
            IsReadyToEndTurn = !IsReadyToEndTurn;
            MyConsole.Print($"[准备结束状态切换] {characterName} {IsReadyToEndTurn}", MessageColor.Yellow);
            if (IsReadyToEndTurn){
                ReadyToEndEvent?.Invoke();
            }else{
                CancelReadyToEndEvent?.Invoke();
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
            var order = other.property.DEX.Value.CompareTo(property.DEX.Value);
            if (order != 0) return order;
            // // 其次阵营靠前的优先行动
            // order = Faction.Value.CompareTo(other.Faction.Value);
            // if (order != 0) return order;
            // 最后随机
            return Random.Range(0, 2) == 0 ? -1 : 1;
        }
    }
}