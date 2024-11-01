using System.Diagnostics.CodeAnalysis;
using Entity;
using UnityEngine;

namespace GamePlaySystem
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
        private static int GetAdjustValue(int prop) => (prop - AVG_PROP) >> 1;
        
        public static int CalculateHp(CharacterProperty property)
        {
            int targetHP = property.BaseHP.Value + property.Level.Value * (5 + GetAdjustValue(property.CON.Value) * 3);
            return Mathf.Max(1, targetHP);
        }

        public static int CalculateWalkRange(CharacterProperty characterProperty)
        {
            return 5 + GetAdjustValue(characterProperty.DEX.Value);
        }
    }
}