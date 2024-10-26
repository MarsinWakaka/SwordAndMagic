using System;
using BattleSystem.FactionSystem;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class GameToolWindow : ScriptableWizard
    {
        public int entityID = 20001;
        public Vector2 charPosition;
        public FactionType factionType;
        
        [MenuItem("自定义工具/部署阶段/创建角色")]
        private static void CreateCharacter()
        {
            // 窗体标题，按钮名称
            ScriptableWizard.DisplayWizard<GameToolWindow>("创建角色", "确认", "应用");
        }
        
        private void OnWizardUpdate()
        {
            helpString = "请在部署阶段使用";
        }

        private void OnWizardCreate()
        {
            // 创建新角色
            FactorySystem.FactoryManager.Instance.CreateCharacter(factionType, entityID, charPosition);
        }
        
        private void OnWizardOtherButton()
        {
            FactorySystem.FactoryManager.Instance.CreateCharacter(factionType, entityID, charPosition);
        }
    }
}