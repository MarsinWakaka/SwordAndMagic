using GamePlaySystem.EmporiumSystem;
using MyEventSystem;
using UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSystem
{
    public class BattleScene : IScene
    {
        private readonly string sceneName = "BattleScene";
        private readonly int levelIndex;
        public BattleScene(int levelIndex)
        {
            this.levelIndex = levelIndex;
        }
        
        public void Init()
        {
            // 初始化战斗场景
            var loadHandle = SceneManager.LoadSceneAsync(sceneName);
            if (loadHandle != null)
                loadHandle.completed += operation =>
                {
                    // TODO 后续转移到存档类里
                    PlayerData.Gold.Value = 50;
                    EventCenter<GameEvent>.Instance.Invoke(GameEvent.GameResourceLoadStart, levelIndex);
                };
            else
            {
                Debug.LogError($"Load Scene Failed，请检查场景名字是否正确 {sceneName}");
            }
        }
        
        public void Release()
        {
            // 释放场景
            UIManager.Instance.ClearPanel();    // 清空UI（战斗面板以及战斗结束面板）
            EventCenter<GameEvent>.Instance.Clear();
        }
    }
}