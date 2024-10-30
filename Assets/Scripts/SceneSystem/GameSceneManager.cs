using GamePlaySystem.EmporiumSystem;
using MyEventSystem;
using UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSystem
{
    public interface IScene
    {
        public void Init();
        public void Release();
    }
    
    public class MainScene : IScene
    {
        private readonly string sceneName = "MainScene";
        public void Init()
        {
            // 初始化开始场景
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                UIManager.Instance.PushPanel(PanelType.MainMenusPanel, null);
            }
            else
            {
                var loadHandle = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                if (loadHandle != null)
                    loadHandle.completed += operation =>
                    {
                        UIManager.Instance.PushPanel(PanelType.MainMenusPanel, null);
                    };
                else
                {
                    Debug.LogError($"Load Scene Failed，请检查场景名字是否正确 {sceneName}");
                }
            }
        }
        
        public void Release()
        {
            // 释放场景
            UIManager.Instance.PopPanel();
        }
    }
    
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
                    EventCenter<GameStage>.Instance.Invoke(GameStage.GameResourceLoadStart, levelIndex);
                };
            else
            {
                Debug.LogError($"Load Scene Failed，请检查场景名字是否正确 {sceneName}");
            }
        }
        
        public void Release()
        {
            // 释放场景
            UIManager.Instance.PopPanel();
            EventCenter<GameStage>.Instance.Clear();
        }
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameSceneManager
    {
        private static IScene _currentScene;

        public static void LoadScene(IScene scene)
        {
            _currentScene?.Release();
            _currentScene = scene;
            _currentScene.Init();
        }
    }
}