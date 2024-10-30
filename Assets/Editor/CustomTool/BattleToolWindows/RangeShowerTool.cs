using GamePlaySystem.RangeDisplay;
using MyEventSystem;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomTool.BattleToolWindows
{
    public class RangeShowerTool : ScriptableWizard
    {
        [MenuItem("自定义工具/战斗阶段/显示")]
        private static void ShowRange()
        {
            // 窗体标题，按钮名称
            DisplayWizard<RangeShowerTool>("显示范围", "确认", "应用");
        }
        
        [Header("显示范围 (5x5)")]
        public RangeType rangeType;
        [Range(0, 20)]
        public int centerX;
        [Range(0, 20)]
        public int centerY;
        [Range(0, 10)]
        public int range;
        // private readonly int[,] Positions = new int[5, 5];

        private void OnWizardCreate()
        {
            EventCenter<GameEvent>.Instance.Invoke(
                GameEvent.RangeOperation, 
                rangeType, new Vector2(centerX, centerY), range );
        }
        
        private void OnWizardOtherButton()
        {
            EventCenter<GameEvent>.Instance.Invoke(
                GameEvent.RangeOperation, 
                rangeType, new Vector2(centerX, centerY), range );
        }

        private void OnEnable()
        {
            rangeType = (RangeType) EditorPrefs.GetInt("ImpactType", 0);
            centerX = EditorPrefs.GetInt("centerX", 2);
            centerY = EditorPrefs.GetInt("centerY", 2);
            range = EditorPrefs.GetInt("ShowRange", 2);
            // for (var x = 0; x < 5; x++) {
            //     for (var y = 0; y < 5; y++) {
            //         if (Math.Abs(x - centerX) + Math.Abs(y - centerY) <= 2) {
            //             Positions[x, y] = 1;
            //         }
            //     }
            // }
        }

        private void OnWizardUpdate()
        {
            EditorPrefs.SetInt("ImpactType", (int) rangeType);
            EditorPrefs.SetInt("centerX", centerX);
            EditorPrefs.SetInt("centerY", centerY);
            EditorPrefs.SetInt("ShowRange", range);
        }
    }
}