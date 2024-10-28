using System.Collections;
using System.IO;
using MyEventSystem;
using UnityEngine;

namespace GamePlaySystem.LevelData
{
    public class LevelDataManager : MonoBehaviour
    {
        private ILevelDataProcessor dataProcessor;
        
        protected void Awake()
        {
            dataProcessor = ServiceLocator.Get<ILevelDataProcessor>();
        }

        // 由GameStateManager调用
        public void OnLoadLevelResourceStart(int levelIndex)
        {
            StartCoroutine(LoadLevelResource(levelIndex));
        }

        private IEnumerator LoadLevelResource(int index)
        {
            // TODO 制定关卡数据格式LevelData，采用JSON格式存储信息（重写思考游戏的玩法）。
            // 开始加载资源
            // 1、加载地形资源
            // 通过制作地图Prefab的方式然后加载
            ReadLevelFile(index);

            // 2、加载单位资源
            // 也是通过Prefab，制作实体包，单位被加载时，向实体管理器注册自己

            // 3、加载演出资源
            // 通过制作Prefab，然后加载

            // 资源加载完毕
            EventCenter<GameStage>.Instance.Invoke(GameStage.GameResourceLoadEnd);
            yield return null;
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
// 