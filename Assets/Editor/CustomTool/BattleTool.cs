using BattleSystem.FactionSystem;
using Entity;
using Entity.Character;
using Entity.Unit;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomTool
{
    public static class BattleTool
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
        
        [MenuItem("自定义工具/战斗场景/角色AP恢复", false, 2)]
        public static void RecoverAP(MenuCommand menuCommand)
        {
            Debug.Log("Selected: " + Selection.activeGameObject.name);
            var character = Selection.activeGameObject.GetComponent<Character>();
            if (character != null){
                character.property.AP.Value = CharacterProperty.AP_MAX;
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
