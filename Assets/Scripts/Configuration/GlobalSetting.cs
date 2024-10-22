using UnityEngine;

namespace Configuration
{
    // 考虑做成配置文件，方便热更
    public static class GlobalSetting
    {
        public static string PersistentSavePath => Application.persistentDataPath + "/Save/";
        public static string PersistentConfigPath => Application.persistentDataPath + "/Config/";
        public static string PersistentAssetBundlePath => Application.persistentDataPath + "/AssetBundle/";
        public static string StreamingLevelPath => Application.streamingAssetsPath + "/Level/";
        
        public static string UIPanelPath => "UI/UIPanel/";
    }
}