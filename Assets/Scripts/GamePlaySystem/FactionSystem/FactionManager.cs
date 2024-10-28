using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Utility.Singleton;

namespace BattleSystem.FactionSystem
{
    /// <summary>
    /// 注意不要出现相互对立的NPC阵营，否则会导致无法判断敌我关系
    /// </summary>

    public enum FactionType
    {
        Player,
        Enemy,
    }
    
    [Flags]
    public enum Relationship
    {
        Self = 1 << 0,
        Ally = 1 << 1,
        Neutral = 1 << 2,
        Hostile = 1 << 3,
        AttackableObject = 1 << 4,
        Ground = 1 << 5,
    }
    
    public class FactionManager : SingletonMono<FactionManager>
    {
        Dictionary<FactionType, Color> factionColors = new()
        {
            {FactionType.Player, new Color(0.26f, 0.71f, 0.24f)},
            {FactionType.Enemy, new Color(0.87f, 0.27f, 0.14f)},
        };
        
        public Color GetFactionColor(FactionType factionType)
        {
            return factionColors[factionType];
        }
        
        protected override void Awake()
        {
            base.Awake();
        }
    }
}