using System.Collections.Generic;
using System.IO;
using MyEventSystem;
using UnityEngine;

namespace GamePlaySystem.LevelData
{
    // public class LevelData
    // {
    //     public List<GridData> MapData;
    //     
    //     public struct GridData
    //     {
    //         Vector2Int position;
    //         long data;          // 后10位bit表示地形类型，后10位表示对象类型
    //     }
    // }
    
    public class LevelDataManager
    {
        private ILevelDataProcessor dataProcessor;
        
        public LevelDataManager()
        {
            dataProcessor = ServiceLocator.Get<ILevelDataProcessor>();
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
            EventCenter<GameStage>.Instance.Invoke(GameStage.GameResourceLoadEnd);
        }

        private void ReadLevelFile(int index)
        {
            // 通过地形索引加载地形数据
            var dataPath = $"{ApplicationRoot.Instance.Config.levelDataPath}/level_{index}.level";
            if (File.Exists(dataPath))
                dataProcessor.LoadLevelData(File.ReadAllText(dataPath));
            else
                Debug.LogError($"Data file not found: {dataPath}");
        }

        public void OnLoadLevelResourceEnd()
        {
            // 通知开始场景演出
            EventCenter<GameStage>.Instance.Invoke(GameStage.ScenarioStart);
        }
    }
}