using BattleSystem.FactionSystem;
using Entity;
using GamePlaySystem.FactionSystem;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomTool.BattleToolWindows
{
    public class CharacterCreateTool : ScriptableWizard
    {
        [MenuItem("自定义工具/部署阶段/创建角色")]
         private static void CreateCharacter()
         {
             // 窗体标题，按钮名称
             ScriptableWizard.DisplayWizard<CharacterCreateTool>("创建角色", "确认", "应用");
         }
         public int entityID;
         public Vector2 charPosition;
         public FactionType factionType;


         private void OnEnable()
         {
             entityID = EditorPrefs.GetInt("entityID", 20001);
             charPosition = new Vector2(
                 EditorPrefs.GetFloat("charPositionX", 0), 
                 EditorPrefs.GetFloat("charPositionY", 0)
             );
             factionType = (FactionType) EditorPrefs.GetInt("factionType", 0);
         }

         private void OnWizardUpdate()
         {
             helpString = "请在部署阶段使用";
             EditorPrefs.SetInt("entityID", entityID);
             EditorPrefs.SetFloat("charPositionX", charPosition.x);
             EditorPrefs.SetFloat("charPositionY", charPosition.y);
             EditorPrefs.SetInt("factionType", (int) factionType);
         }

         private void OnWizardCreate()
         {
             // 创建新角色
             ServiceLocator.Get<IEntityFactory>().CreateCharacter(entityID, charPosition, factionType);
         }
         
         private void OnWizardOtherButton()
         {
             ServiceLocator.Get<IEntityFactory>().CreateCharacter(entityID, charPosition, factionType);
         }
    }
}