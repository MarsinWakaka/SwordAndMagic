using System;
using System.Collections.Generic;
using GamePlaySystem.CharacterClassSystem;
using GamePlaySystem.SkillSystem;

// ReSharper disable InconsistentNaming
namespace Data
{
    [Serializable]
    public class CharacterData
    {
        public string Name;
        
        public int CON;
        public int STR;
        public int INT;
        public int PER;
        public int DEX;
        public int BaseHp;
        public int BaseWalkRange;

        public int HP_Max;
        public int DEF_Max;
        public int MDEF_Max;
        public int WR_Max;
        
        public int HP;
        public int DEF;
        public int MDEF;
        public int RWR;
        
        public int AP;
        public int SP;
        
        public int LV;
        public List<CharacterClassData> classTable;
        public List<SkillSlot> Skills;
        
        public int HostileScoreFactor;
        public int HostileImpactRange;
        public int FriendlyScoreFactor;
        public int FriendlyImpactRange;
    }
}