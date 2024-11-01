using System.Collections.Generic;
using System.IO;
using Configuration;
using MyEventSystem;
using UnityEngine;

namespace GamePlaySystem.LevelData
{
    public class LevelData
    {
        public List<int> MapData;   // 9位数字，每三位分别代表(X,Y,TileType)
    }
    
    public class LevelDataManager
    {
        private ILevelDataProcessor _dataProcessor;
        
        public LevelDataManager(ILevelDataProcessor dataProcessor)
        {
            _dataProcessor = dataProcessor;
        }

        // 由GameStateManager调用
        public void OnLoadLevelResourceStart(int levelIndex)
        {
            // 先加载所有对象数据和地图数据
            OnGenerateLevelData(levelIndex);
        }
        
        public void OnGenerateLevelData(int levelIndex)
        {
            // 生成地形数据
            ReadLevelFile(levelIndex);
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.GameResourceLoadEnd);
        }

        private void ReadLevelFile(int index)
        {
            // 通过地形索引加载地形数据
            var config = ServiceLocator.Get<IConfigService>().ConfigData;
            var dataPath = $"{config.levelDataPath}/level_{index}.level";
            if (File.Exists(dataPath))
                _dataProcessor.LoadLevelData(File.ReadAllText(dataPath));
            else
                Debug.LogError($"Property file not found: {dataPath}");
        }

        public void OnLoadLevelResourceEnd()
        {
            // 通知开始场景演出
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.ScenarioStart);
        }
    }
}