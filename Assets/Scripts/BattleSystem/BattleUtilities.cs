using System;
using System.Diagnostics.CodeAnalysis;
using BattleSystem.Entity;
using Entity.Character;

namespace BattleSystem
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] // Disable naming rule
    public static class BattleUtilities
    {
        // 考虑提取到配置文件中
        private const int AVG_PROP = 10;
        private const int BASE_AP = 3;
        private const int BASE_SP = 1;
        
        /// <summary>
        /// 获得该属性值与与属性基本值的差
        /// </summary>
        private static int GetDeltaValue(int prop) => prop - AVG_PROP;
        
        public static int CalculateHp(CharacterProperty property)
        {
            return property.BaseHP.Value + GetDeltaValue(property.CON.Value) * 5 + property.Level.Value * 8;
        }

        public static int CalculateWalkRange(CharacterProperty characterProperty)
        {
            return 5 + GetDeltaValue(characterProperty.DEX.Value) / 2;
        }
    }
}