using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GamePlaySystem;
using GamePlaySystem.CharacterClassSystem;
using GamePlaySystem.SkillSystem;
using UnityEngine;
using Utility;

namespace Data
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] // Disable naming rule
    [CreateAssetMenu(menuName = "创建角色数据", fileName = "New Character Property")]
    public class CharacterProperty : ScriptableObject
    {
        // !!! 添加新的属性时，需要在DeepCopy中添加拷贝
        [Header("【角色基本信息】")]
        [SerializeField] public string characterName;
        [SerializeField] public int sellPrice;
        [Range(1, 12)]
        [SerializeField] private int LV;
        public List<BaseSkill> Skills;
        [Tooltip("防御面板-护盾存在期间，角色不会被暴击；护盾不能通过治疗恢复")]
        [SerializeField] private int Defence_Max;
        [SerializeField] private int MagicDefence_Max;
        
        [Header("角色一级属性")]
        [Tooltip("体质: 影响角色倒下前能承受多少伤害(角色获得额外体质调整值的生命值)")]
        [Range(1, 24)] 
        [SerializeField] private int Constitution = 10;
        [Tooltip("力量：增强武器的操纵能力(影响物理攻击的命中以及伤害)")]
        [Range(1, 24)] 
        [SerializeField] private int Strength = 10;
        [Tooltip("智力：增强魔法的操纵能力(影响魔法攻击的命中以及伤害)")]
        [Range(1, 24)] 
        [SerializeField] private int Intelligence = 10;
        // [Tooltip("感知: 识破敌人行为(影响暴击率)的能力--角色攻击时，如果触发识破，造成两倍伤害; 角色受击时，如果触发识破，只受到一半伤害")]
        [Tooltip("感知: 识破敌人行为、强化治愈效果、提升角色心智类技能的命中--角色回合开始时，对场上的敌人进行感知检测，如果发现敌人的弱点，则对敌人施加<弱点标记>" +
                 "弱点标记：带有弱点标记的敌人受到的精准类型的伤害时，额外承受50%的伤害，弱点标记在敌人的行动开始时消失")]
        [Range(1, 24)] 
        [SerializeField] private int Perception = 10;
        [Tooltip("敏捷值：影响先攻、每回合依据<敏捷调整值>额外获得移动距离、影响角色的闪避能力")]
        [Range(1, 24)] 
        [SerializeField] private int Dexterity = 10;
        
        // [Header("种族")]
        [SerializeField] private int BaseHp;
        [SerializeField] private int BaseWalkRange;
        
        [Header("TODO 职业（class）情况：职业会影响AI的行为")]
        public List<CharacterClassData> classTable;
        // 角色的AI参数，从而定义AI操纵下不同角色的行为偏好
        // 例如：坦克更倾向于冲向敌人，法师更倾向于远离敌人，治疗更倾向于靠近友军
        [Header("角色行为参数(仅影响AI操作下的行为)")] 
        [Tooltip("敌对分数因子：敌对分数因子与AI冲向敌人的倾向正比")]
        public float HostileScoreFactor = 100;
        [Tooltip("敌对影响范围：敌人影响AI倾向的影响范围")]
        public int HostileImpactRange = 5;
        [Tooltip("友好分数因子：友好分数因子与AI呆在友军附近的倾向成正比(此参数为负数时，可以防止AI扎堆)")]
        public float FriendlyScoreFactor = -50;
        [Tooltip("友好影响范围：友军影响AI倾向的影响范围")]
        public int FriendlyImpactRange = 3;
        
        [Header("行动点 与 技能点")] 
        public const int AP_MAX = 6; // Max Action Points，最大行动点
        public const int SP_MAX = 3; // Max Skill Points，最大技能 
        
        public BindableProperty<int> Level;
        public BindableProperty<int> BaseHP;
        // 一级属性
        public BindableProperty<int> CON;      // Constitution，体质
        public BindableProperty<int> STR;      // Strength，力量
        public BindableProperty<int> INT;      // Intelligence，智力
        public BindableProperty<int> PER;      // Perception，感知
        public BindableProperty<int> DEX;      // Dexterity，敏捷
        
        // 二级显性属性
        public BindableProperty<int> HP_MAX;  // Max Health Points，最大生命值
        public BindableProperty<int> DEF_MAX; // Max Defense，最大防御力
        public BindableProperty<int> MDEF_MAX;// Max Magic Defense，最大魔法防御力
        public BindableProperty<int> WR_MAX; // 每回合的基础移动范围
        
        public BindableProperty<int> HP;      // Health Points，生命值
        public BindableProperty<int> DEF;     // Defense，物理防御力
        public BindableProperty<int> MDEF;    // Magic Defense，魔法防御力
        public BindableProperty<int> RWR;     // 剩余移动范围
        public BindableProperty<int> AP;      // Action Points，行动点
        public BindableProperty<int> SP;      // Skill Points，技能点
        
        public void LoadPlayerData(CharacterData data)
        {
            // 这里直接进行BindableProperty的赋值，就行
            // 如果是保存数据，也是直接将BindableProperty的Value保存
        }
        
        public void Initialize()
        {
            Level = new BindableProperty<int>(LV);
            BaseHP = new BindableProperty<int>(BaseHp);
            
            // 用于UI监听 以及部分属性(如HP)的更新
            CON = new BindableProperty<int>(Constitution);
            STR = new BindableProperty<int>(Strength);
            INT = new BindableProperty<int>(Intelligence);
            PER = new BindableProperty<int>(Perception);
            DEX = new BindableProperty<int>(Dexterity);
            
            HP_MAX = new BindableProperty<int>(BattleUtilities.CalculateHp(this));
            DEF_MAX = new BindableProperty<int>(Defence_Max);
            MDEF_MAX = new BindableProperty<int>(MagicDefence_Max);
            WR_MAX = new BindableProperty<int>(BattleUtilities.CalculateWalkRange(this));
            
            // 初始化当前属性
            HP = new BindableProperty<int>(HP_MAX.Value);
            DEF = new BindableProperty<int>(DEF_MAX.Value);
            MDEF = new BindableProperty<int>(MDEF_MAX.Value);
            RWR = new BindableProperty<int>(WR_MAX.Value);
            AP = new BindableProperty<int>(AP_MAX);
            SP = new BindableProperty<int>(SP_MAX);
            
            // 添加监听
            Level.OnValueChanged += level => HP_MAX.Value = BattleUtilities.CalculateHp(this);
            BaseHP.OnValueChanged += baseHp => HP_MAX.Value = BattleUtilities.CalculateHp(this);
            CON.OnValueChanged += (con) =>
            {
                int oldHpMax = HP_MAX.Value;
                HP_MAX.Value = BattleUtilities.CalculateHp(this);
                HP.Value += HP_MAX.Value - oldHpMax;
            };
        }
        
        public CharacterProperty DeepCopy()
        {
            var copy = CreateInstance<CharacterProperty>();
            copy.characterName = characterName;
            copy.sellPrice = sellPrice;
            copy.LV = LV;
            copy.BaseHp = BaseHp;
            copy.Defence_Max = Defence_Max;
            copy.MagicDefence_Max = MagicDefence_Max;
            copy.Constitution = Constitution;
            copy.Strength = Strength;
            copy.Intelligence = Intelligence;
            copy.Perception = Perception;
            copy.Dexterity = Dexterity;
            copy.HostileScoreFactor = HostileScoreFactor;
            copy.HostileImpactRange = HostileImpactRange;
            copy.FriendlyScoreFactor = FriendlyScoreFactor;
            copy.FriendlyImpactRange = FriendlyImpactRange;
            return copy;
        }
    }
}