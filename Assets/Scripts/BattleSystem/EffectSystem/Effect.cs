using System;
using BattleSystem.BUFFSystem;
using BattleSystem.Entity.Character;
using Entity.Character;
using UnityEngine;
using UnityEngine.Serialization;

namespace BattleSystem.EffectSystem
{
    /// <summary>
    /// 考虑替换为Command模式
    /// </summary>
    public class Effect
    {
        public virtual void ApplyEffect()
        {
            throw new NotImplementedException();
        }
    }
    
    // 物理伤害效果
    public class DamageEffect : Effect
    {
        private Character target;
        private Character caster;

        public override void ApplyEffect()
        {
            target.TakeDamage(caster.property.STR.Value, DamageType.Physical);
        }
    }
    
    // 治疗效果
    public class HealEffect : Effect
    {
        public int HealAmount;
        public int Duration;

        public override void ApplyEffect()
        {
            throw new NotImplementedException();
        }
    }
    
    // 燃烧效果
    public class BurnEffect : Effect
    {
        public int duration;
        
        public override void ApplyEffect()
        {
            // target.AddBuff(new BurnBuff(target, duration));
        }
    }
}