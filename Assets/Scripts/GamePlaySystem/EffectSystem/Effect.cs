using System;
using Entity;
using GamePlaySystem.SkillSystem;

namespace GamePlaySystem.EffectSystem
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
            target.TakeDamage(caster.Property.STR.Value, DamageType.Physical);
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