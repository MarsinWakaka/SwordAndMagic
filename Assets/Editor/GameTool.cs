using BattleSystem;
using BattleSystem.FactionSystem;
using Entity.Character;
using Entity.Tiles;
using FactorySystem;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class GameTool
    {
        // %=ctrl  #=shift  &=alt
        [MenuItem("自定义工具/战斗场景/创建无用之人(请选择地块后使用) &D", true, 1)]
        public static bool CreateCharacter()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null){
                Debug.LogWarning("请选中一个地形块再进行部署");
                return false;
            }
            return true;
        }
        
        [MenuItem("自定义工具/战斗场景/创建无用之人(请在部署阶段使用) &D", false, 1)]
        public static void CreateCharacter(MenuCommand menuCommand)
        {
            var tile = Selection.activeGameObject.GetComponent<Tile>();
            if (tile == null){
                Debug.LogWarning("请选中一个地形块再进行部署");
                return;
            }
            var factionType = FactionType.Player;
            var position = tile.transform.position;
            var entityID = 20001;
            // 创建新角色
            FactoryManager.Instance.CreateCharacter(factionType, entityID, position);
        }
        
        [MenuItem("自定义工具/战斗场景/角色AP恢复", false, 2)]
        public static void RecoverAP(MenuCommand menuCommand)
        {
            Debug.Log("Selected: " + Selection.activeGameObject.name);
            var character = Selection.activeGameObject.GetComponent<Character>();
            if (character != null){
                character.property.AP.Value = CharacterProperty.AP_MAX;
            }
        }
        
        // 请注意开头为：CONTEXT/
        [MenuItem("CONTEXT/Character/RecoverAllStatus")]
        public static void RecoverAllStatus(MenuCommand menuCommand)
        {
            Character character = menuCommand.context as Character;
            if (character != null){
                var property = character.property;
                Undo.RecordObject(property, "Recover All Status");
                property.AP.Value = CharacterProperty.AP_MAX;
                property.SP.Value = CharacterProperty.SP_MAX;
                property.HP.Value = property.HP_MAX.Value;
                property.RWR.Value = property.WR_MAX.Value;
            }
        }
        
        [MenuItem("自定义工具/战斗场景/摧毁选中角色")]
        public static void DestroyCharacter()
        {
            Character character = Selection.activeGameObject.GetComponent<Character>();
            if (character != null){
                // Object.DestroyImmediate(character.gameObject); // 此操作无法撤销
                Undo.DestroyObjectImmediate(character.gameObject);
            }else{
                Debug.LogError("选中的对象没有挂载角色组件");
            }
        }
    }
}
