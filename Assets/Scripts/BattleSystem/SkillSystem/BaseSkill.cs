using System;
using System.Diagnostics.CodeAnalysis;
using BattleSystem.EffectSystem;
using Entity;
// using BattleSystem.Scope;
using Entity.Character;
using UnityEngine;
using UnityEngine.Serialization;

// using BattleSystem.Faction;

namespace BattleSystem.SkillSystem
{
    public enum RangeType
    {
        Self,
        Single,
        Circle,
        Rectangle,
        Line,
        Cross,
        Custom,
        // All
    }
    
    [Flags]
    public enum CanImpactFactionType
    {
        Self = 1,
        Ally = 2,
        Enemy = 4,
        All = 7,
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    // [CreateAssetMenu(menuName = "Skill/BaseSkill", fileName = "New skill")]
    public abstract class BaseSkill : ScriptableObject
    {
        private Character caster;
        
        [Header("技能基本信息")]
        public int id;
        public string skillName;
        // 改为GetDescription，具体描述由子类实现
        // public string description;
        public Sprite skillIcon;
        [Header("技能效果")]
        public Effect[] effects;
        
        [Header("施法范围 & 影响范围 & 作用对象")]
        public int range;
        public RangeType rangeType;
        [Header("作用对象 & 作用数量 & 是否可重复作用")]
        public EntityType canImpactEntityType;
        public CanImpactFactionType canImpactFactionType;   // 仅在canImpactEntityType为有Character时有效
        public int maxTargetCount;
        public bool canSameTarget;
        [Header("技能限制")]
        [Tooltip("是否需要专注")]
        public bool isChanneling;
        public int coolDown;
        [Header("技能消耗")]
        public int AP_Cost;
        public int SP_Cost;
        //
        // private IScope _releaseScope;
        // private IImpactScope _impactScope;
        //
        // protected void Init(IScope releaseScope, IImpactScope impactScope)
        // {
        //     if (releaseScope == null) Debug.LogError("ReleaseScope is null");
        //     if (impactScope == null) Debug.LogError("ImpactScope is null");
        //     
        //     _releaseScope = releaseScope;
        //     _impactScope = impactScope;
        // }

        /// <summary>
        /// 通过曼哈顿距离判断目标点是否在施法范围内
        /// </summary>
        public abstract bool isTargetInRange(Vector2 targetPosition); // => _releaseScope.IsInScope(param);
        
        /// <summary>
        /// 显示技能范围
        /// </summary>
        public abstract Vector2[] GetReleaseScope(Vector2 targetPosition);
        
        /// <summary>
        /// 判断受到影响的目标点，可被子类重写，因此可以自定义法术影响范围
        /// </summary>
        /// <returns>返回受到影响的目标点集合</returns>
        public abstract Vector2[] GetImpactScope(Vector2 targetPosition); // => _impactScope.GetImpactScope(param);
        
        /// <summary>
        /// 法术执行
        /// step1：获取技能范围
        /// step2：获取影响范围
        /// step3：获取受影响的对象(Character、Tile)
        /// step4：执行技能效果
        /// </summary>
        public abstract void Execute(Character caster, BaseEntity[] targets);
    }
}