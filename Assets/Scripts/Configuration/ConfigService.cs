using System;
using Utility.SerializeTool;

namespace Configuration
{
    // 考虑做成配置文件，方便热更
    [Serializable]
    public class ConfigData
    {
        public string configVersion;
        public int levelCount;
        public string levelDataParser;
        public string levelDataPath;
        public string levelDataPrefix;
        public string levelDataSuffix;
        public string uiPanelPath;
        public string characterPrefabPath;
        public string tilePrefabPath;
    }

    public interface IConfigService
    {
        ConfigData ConfigData { get; }
        void ReloadConfig(string configPath, ISerializeTool serializeTool);
    }

    public class ConfigService : IConfigService
    {
        public ConfigData ConfigData { get; private set; }

        public ConfigService(string configPath, ISerializeTool serializeTool)
        {
            ConfigData = LoadConfig(configPath, serializeTool);
        }

        public void ReloadConfig(string configPath, ISerializeTool serializeTool)
        {
            ConfigData = LoadConfig(configPath, serializeTool);
        }

        private ConfigData LoadConfig(string configPath, ISerializeTool serializeTool)
        {
            return serializeTool.Deserialize<ConfigData>(configPath);
        }
    }
}