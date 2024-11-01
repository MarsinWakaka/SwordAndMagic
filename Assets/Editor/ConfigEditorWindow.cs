using Configuration;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Editor
{
    public class ConfigEditorWindow : EditorWindow
    {
        [Header("Config File Path")]
        [SerializeField] private string filePath = "Assets/ConfigData.json"; // 默认文件路径
        [Header("配置文件内容")]
        private ConfigData configData;

        [MenuItem("自定义工具/编辑全局配置文件")]
        public static void ShowWindow()
        {
            GetWindow<ConfigEditorWindow>("Config Editor");
        }

        private void OnEnable()
        {
            LoadConfigData();
        }

        private void OnGUI()
        {
            GUILayout.Label("Edit Config Property", EditorStyles.boldLabel);

            // 输入字段
            configData.configVersion = EditorGUILayout.TextField("Config Version", configData.configVersion);
            configData.levelCount = EditorGUILayout.IntField("Level Count", configData.levelCount);
            configData.levelDataParser = EditorGUILayout.TextField("Level Property Parser", configData.levelDataParser);
            configData.levelDataPath = EditorGUILayout.TextField("Level Property Path", configData.levelDataPath);
            configData.levelDataPrefix = EditorGUILayout.TextField("Level Property Prefix", configData.levelDataPrefix);
            configData.levelDataSuffix = EditorGUILayout.TextField("Level Property Suffix", configData.levelDataSuffix);
            configData.uiPanelPath = EditorGUILayout.TextField("UI Panel Path", configData.uiPanelPath);
            configData.characterPrefabPath =
                EditorGUILayout.TextField("Character Prefab Path", configData.characterPrefabPath);
            configData.tilePrefabPath = EditorGUILayout.TextField("Tile Prefab Path", configData.tilePrefabPath);

            // 保存按钮
            if (GUILayout.Button("Save Config"))
            {
                SaveConfigData();
            }
            if (GUILayout.Button("Load Config"))
            {
                LoadConfigData();
            }
        }

        private void LoadConfigData()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                configData = JsonUtility.FromJson<ConfigData>(json);
            }
            else
            {
                configData = new ConfigData(); // 如果文件不存在，初始化为默认值
            }
        }

        private void SaveConfigData()
        {
            string json = JsonUtility.ToJson(configData, true);
            File.WriteAllText(filePath, json);
            AssetDatabase.Refresh(); // 刷新资源数据库
            Debug.Log("Config saved to " + filePath);
        }
    }
}