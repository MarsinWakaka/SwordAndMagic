using System;
using UnityEngine;

namespace Configuration
{
    // 考虑做成配置文件，方便热更
    [Serializable]
    public class ConfigData
    {
        /// <summary>
        /// 关卡配置
        /// </summary>
        public int levelCount;
        public string levelDataParser;
        public string levelDataPath;
        public string levelDataPrefix;
        public string levelDataSuffix;
        
        /// <summary>
        /// UI配置
        /// </summary>
        public string uiPanelPath;
        
        public string characterPrefabPath;
        public string tilePrefabPath;
    }
}