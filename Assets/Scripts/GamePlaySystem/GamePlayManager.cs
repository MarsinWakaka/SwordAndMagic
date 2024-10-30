using BattleSystem.ScenarioSystem;
using ConsoleSystem;
using GamePlaySystem.DeploySystem;
using GamePlaySystem.LevelData;
using MyEventSystem;
using UISystem;
using UnityEngine;

namespace GamePlaySystem
{
    public class GamePlayManager : MonoBehaviour
    {
        private LevelDataManager levelDataManager;
        [SerializeField] private ScenarioManager scenarioManager;
        // TODO 将其从MonoBehaviour改为其它。前提是解决部署资源获取问题
        [SerializeField] private DeployManager deployManager;
        [SerializeField] private BattleManager battleManager;
        
        private const MessageColor FontColor = MessageColor.Magenta;
        
        private void Awake()
        {
            levelDataManager = new LevelDataManager();
            // TODO 读取资源后往DeployManager里面传递
            EventCenter<GameStage>.Instance.AddListener<int>(GameStage.GameResourceLoadStart, OnGameResourceLoadStart);
        }

        private void OnGameResourceLoadStart(int sceneIndex)
        {
            MyConsole.Print("【游戏资源加载开始】", FontColor);
            EventCenter<GameStage>.Instance.AddListener(GameStage.GameResourceLoadEnd, OnGameResourceLoadEnd);
            
            // 测试加载关卡1
            levelDataManager.OnLoadLevelResourceStart(sceneIndex);// 游戏资源加载开始 --执行完毕-> 游戏资源加载结束
        }
        
        private void OnGameResourceLoadEnd()
        {
            MyConsole.Print("【游戏资源加载结束】", FontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.GameResourceLoadEnd, OnGameResourceLoadEnd);
            EventCenter<GameStage>.Instance.AddListener(GameStage.ScenarioStart, OnScenarioStart);
            
            levelDataManager.OnLoadLevelResourceEnd();// 游戏资源加载结束 --执行完毕-> 演出开始
        }
        
        private void OnScenarioStart()
        {
            MyConsole.Print("【演出开始】", FontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.ScenarioStart, OnScenarioStart);
            EventCenter<GameStage>.Instance.AddListener(GameStage.ScenarioEnd, OnScenarioEnd);
            
            scenarioManager.OnScenarioStart(); // 场景演出开始 --执行完毕-> 场景演出结束
        }
        
        private void OnScenarioEnd()
        {
            MyConsole.Print("【演出结束】", FontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.ScenarioEnd, OnScenarioEnd);
            EventCenter<GameStage>.Instance.AddListener(GameStage.PlayerDeployedStart, OnPlayerDeployedStart);
            
            scenarioManager.OnScenarioEnd(); // 场景演出结束 --执行完毕-> 玩家部署开始
        }
        
        private void OnPlayerDeployedStart()
        {
            MyConsole.Print("【玩家部署开始】", FontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.PlayerDeployedStart, OnPlayerDeployedStart);
            EventCenter<GameStage>.Instance.AddListener(GameStage.PlayerDeployedEnd, OnPlayerDeployedEnd);
            
            deployManager.OnDeployStart(); // 玩家部署开始 --执行完毕-> 玩家部署结束
        }
        
        private void OnPlayerDeployedEnd()
        {
            MyConsole.Print("【玩家部署结束】", FontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.PlayerDeployedEnd, OnPlayerDeployedEnd);
            EventCenter<GameStage>.Instance.AddListener(GameStage.BattleStart, OnBattleStart);
            deployManager.OnDeployEnd(); // 玩家部署结束 --执行完毕-> 战斗开始
        }
        
        private void OnBattleStart()
        {
            MyConsole.Print("【战斗开始】", FontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.BattleStart, OnBattleStart);
            EventCenter<GameStage>.Instance.AddListener(GameStage.BattleEnd, OnBattleEnd);
            
            battleManager.OnBattleStartAction(); // 战斗开始 --执行完毕-> 战斗结束
        }

        private void OnBattleEnd()
        {
            MyConsole.Print("【战斗结束】", FontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.BattleEnd, OnBattleEnd);
            
            GamePlayEnd();
        }

        private void GamePlayEnd()
        {
            MyConsole.Print("[游戏结束]", MessageColor.Black);
            UIManager.Instance.PushPanel(PanelType.BattleEndPanel, null);
        }
    }
}