using System;
using Entity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BattleSystem.BUFFSystem
{
    [Flags]
    public enum BuffType
    {
        Burn = 1,
        Bleed = 2,
        Thorn = 4,
    }
    
    public abstract class Buff
    {
        public BuffType BuffType;
        public int Duration;

        public abstract void ApplyBuffEffect();
    }
    
    public class BurnBuff : Buff
    {
        // 考虑替换类型为 IBurnable
        private Character target; 
            
        public BurnBuff(Character target, int duration)
        {
            BuffType = BuffType.Burn;
            // target.OnStartTurnEvent += ApplyBuffEffect;
        }
        
        public override void ApplyBuffEffect()
        {
            // 1 ~ 4
            var damage = Random.Range(1, 5);
            // target.OnTakeDamage(Character.DamageType.Magic, damage);
            Debug.Log($"BurnBuff: {target.characterName} 受到了 {damage} 点火焰伤害");
        }
    }
    
    public class BleedBuff : Buff
    {
        public override void ApplyBuffEffect()
        {
            throw new System.NotImplementedException();
        }
    }
    
    // 你受到伤害时，对攻击者造成等量伤害
    public class ThornBuff : Buff
    {
        public override void ApplyBuffEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}