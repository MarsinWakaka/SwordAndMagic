using System.Collections.Generic;

namespace BattleSystem.SkillSystem
{
    public static class SkillRegistry
    {
        private static readonly Dictionary<int, BaseSkill> SkillDict = new();
        
        public static void RegisterSkill(BaseSkill skill)
        {
            SkillDict.Add(skill.id, skill);
        }
        
        public static BaseSkill GetSkill(int id)
        {
            return SkillDict[id];
        }
    }
}