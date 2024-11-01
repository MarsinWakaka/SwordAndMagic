using System.Collections.Generic;
using Entity;
using GamePlaySystem.SkillSystem;
using UnityEngine;

namespace GamePlaySystem.FactionSystem
{
    /// <summary>
    /// 注意不要出现相互对立的NPC阵营，否则会导致无法判断敌我关系
    /// </summary>

    public enum FactionType
    {
        Player,
        Enemy,
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FactionManager
    {
        private static readonly Dictionary<FactionType, Color> factionColors = new()
        {
            {FactionType.Player, new Color(0.26f, 0.71f, 0.24f)},
            {FactionType.Enemy, new Color(0.87f, 0.27f, 0.14f)},
        };
        
        public static Color GetFactionColor(FactionType factionType)
        {
            return factionColors[factionType];
        }
        
        public static bool CanImpact(Character self, Character target, CanImpactFactionType canImpactFactionType)
        {
            if (self == target) return (canImpactFactionType & CanImpactFactionType.Self) != 0;
            return self.Faction.Value != target.Faction.Value && (canImpactFactionType & CanImpactFactionType.Hostile) != 0 ||
                   self.Faction.Value == target.Faction.Value && (canImpactFactionType & CanImpactFactionType.Ally) != 0;
        }
    }
}