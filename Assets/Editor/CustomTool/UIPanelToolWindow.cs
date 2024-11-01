using System;
using UISystem;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomTool
{
    public enum OperationType
    {
        Push,
        Pop,
    }
    
    public class UIPanelToolWindow : ScriptableWizard
    {
        [Header("注意：操作模式为Pop时，面板类型无效")]
        public OperationType operationType;
        public PanelType panelType;
        
        [MenuItem("自定义工具/UI/管理UI面板")]
        private static void OpenPanelManager()
        {
            // 窗体标题，按钮名称
            ScriptableWizard.DisplayWizard<UIPanelToolWindow>("UI面板管理工具", "关闭工具", "执行");
        }

        private void OnWizardCreate() { }

        private void OnWizardOtherButton()
        {
            switch (operationType)
            {
                case OperationType.Push:
                    UIManager.Instance.PushPanel(panelType, null);
                    break;
                case OperationType.Pop:
                    UIManager.Instance.PopPanel(panelType);
                    break;
                default:
                    Debug.LogWarning($"未知操作类型 {operationType}");
                    break;
            }
        }

        private void OnEnable()
        {
            helpString = "UI面板管理";
            panelType = (PanelType) EditorPrefs.GetInt("panelType", 0);
            operationType = (OperationType) EditorPrefs.GetInt("operationType", 0);
        }

        private void OnWizardUpdate()
        {
            EditorPrefs.SetInt("panelType", (int) panelType);
            EditorPrefs.SetInt("operationType", (int) operationType);
        }
    }
}