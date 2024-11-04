using GamePlaySystem.RangeDisplay;
using GamePlaySystem.TileSystem.Navigation;
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
        
        [Range(0, 20)]
        public int centerX;
        [Range(0, 20)]
        public int centerY;
        [Range(0, 10)]
        public int range;
        // private readonly int[,] Positions = new int[5, 5];

        private void OnWizardCreate()
        {
            var map = ServiceLocator.Get<INavigationService>().GetReachablePositionDict(centerX, centerY, range);
            ServiceLocator.Get<IRangeDisplayService>().ShowMoveRange(map, range);
        }
        
        private void OnWizardOtherButton()
        {
            var map = ServiceLocator.Get<INavigationService>().GetReachablePositionDict(centerX, centerY, range);
            ServiceLocator.Get<IRangeDisplayService>().ShowMoveRange(map, range);
        }

        private void OnEnable()
        {
            centerX = EditorPrefs.GetInt("centerX", 2);
            centerY = EditorPrefs.GetInt("centerY", 2);
            range = EditorPrefs.GetInt("ShowMoveRange", 2);
        }

        private void OnWizardUpdate()
        {
            EditorPrefs.SetInt("centerX", centerX);
            EditorPrefs.SetInt("centerY", centerY);
            EditorPrefs.SetInt("ShowMoveRange", range);
        }
    }
}