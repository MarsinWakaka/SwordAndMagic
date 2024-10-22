using System;
using System.Collections;
using System.IO;
using BattleSystem;
using EventSystem;
using UnityEngine;
using Utility;
using Utility.Singleton;
using GlobalSetting = Configuration.GlobalSetting;

namespace GameResourceSystem
{
    public class GameResourceManager : SingletonMono<GameResourceManager>
    {
        [SerializeField]
        private LevelDataProcessor terrainResourceLoader;

        private void ParseLevelFile(String levelData)
        {
            // 解析地形数据
            terrainResourceLoader.ProcessLevelData(levelData);
        }

        private void ReadLevelFile(int index)
        {
            // 通过地形索引加载地形数据
            string terrianDataPath = GlobalSetting.StreamingLevelPath + $"/level_{index}.level";
            if (File.Exists(terrianDataPath))
            {
                ParseLevelFile(File.ReadAllText(terrianDataPath));
            }
            else
            {
                Debug.LogError($"Terrian data file not found: {terrianDataPath}");
            }
        }

        // 由GameStateManager调用
        public void OnLoadLevelResourceStart(int levelIndex)
        {
            StartCoroutine(LoadLevelResource(levelIndex));
        }

        private IEnumerator LoadLevelResource(int index)
        {
            // 开始加载资源
            // 1、加载地形资源
            // 通过制作地图Prefab的方式然后加载
            ReadLevelFile(index);

            // 2、加载单位资源
            // 也是通过Prefab，制作实体包，单位被加载时，向实体管理器注册自己

            // 3、加载演出资源
            // 通过制作Prefab，然后加载

            // 资源加载完毕
            EventCenter<GameStateEvent>.Instance.Invoke(GameStateEvent.GameStateGameResourceLoadEnd);
            yield return null;
        }

        public void OnLoadLevelResourceEnd()
        {
            // 通知开始场景演出
            EventCenter<GameStateEvent>.Instance.Invoke(GameStateEvent.GameStateScenarioStart);
        }
    }
}