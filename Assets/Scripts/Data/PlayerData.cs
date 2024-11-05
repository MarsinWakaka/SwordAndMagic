using System;
// ReSharper disable InconsistentNaming

namespace Data
{
    [Serializable]
    public class PlayerData
    {
        public string characterName;
        public int sellPrice;
        public int LV;
        public int Defence_Max;
        public int MagicDefence_Max;
        public int Constitution;
        public int Strength;
        public int Intelligence;
        public int Perception;
        public int Dexterity;
        public int BaseHp;
        public int BaseWalkRange;
    }
}