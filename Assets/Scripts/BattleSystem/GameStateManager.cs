using BattleSystem.DeploySystem;
using BattleSystem.ScenarioSystem;
using ConsoleSystem;
using GameResourceSystem;
using MyEventSystem;
using UnityEngine;

namespace BattleSystem
{
    /// <summary>
    /// 这个东西应该被设计为场景管理器的一个战斗场景类，负责游戏的整体流程控制
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        [Header("演出编排")]
        public ScenarioManager scenarioManager;
        [SerializeField] private DeployManager deployManager;

        private void Start()
        {
            // 本质上是一个状态机，每个阶段都有自己的逻辑，OnEnter -> OnExit
            OnGameResourceLoadStart();
        }

        #region 游戏阶段

        private const MessageColor MessageFontColor = MessageColor.Magenta;

        private void OnGameResourceLoadStart()
        {
            MyConsole.Print("【游戏资源加载开始】", MessageFontColor);
            EventCenter<GameStage>.Instance.AddListener(GameStage.GameResourceLoadEnd, OnGameResourceLoadEnd);
            
            // 测试加载关卡1
            GameResourceManager.Instance.OnLoadLevelResourceStart(1);// 游戏资源加载开始 --执行完毕-> 游戏资源加载结束
        }
        
        private void OnGameResourceLoadEnd()
        {
            MyConsole.Print("【游戏资源加载结束】", MessageFontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.GameResourceLoadEnd, OnGameResourceLoadEnd);
            EventCenter<GameStage>.Instance.AddListener(GameStage.ScenarioStart, OnScenarioStart);
            
            GameResourceManager.Instance.OnLoadLevelResourceEnd();// 游戏资源加载结束 --执行完毕-> 演出开始
        }
        
        private void OnScenarioStart()
        {
            MyConsole.Print("【演出开始】", MessageFontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.ScenarioStart, OnScenarioStart);
            EventCenter<GameStage>.Instance.AddListener(GameStage.ScenarioEnd, OnScenarioEnd);
            
            scenarioManager.OnScenarioStart(); // 场景演出开始 --执行完毕-> 场景演出结束
        }
        
        private void OnScenarioEnd()
        {
            MyConsole.Print("【演出结束】", MessageFontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.ScenarioEnd, OnScenarioEnd);
            EventCenter<GameStage>.Instance.AddListener(GameStage.PlayerDeployedStart, OnPlayerDeployedStart);
            
            scenarioManager.OnScenarioEnd(); // 场景演出结束 --执行完毕-> 玩家部署开始
        }
        
        private void OnPlayerDeployedStart()
        {
            MyConsole.Print("【玩家部署开始】", MessageFontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.PlayerDeployedStart, OnPlayerDeployedStart);
            EventCenter<GameStage>.Instance.AddListener(GameStage.PlayerDeployedEnd, OnPlayerDeployedEnd);
            
            deployManager.OnDeployStart(); // 玩家部署开始 --执行完毕-> 玩家部署结束
        }
        
        private void OnPlayerDeployedEnd()
        {
            MyConsole.Print("【玩家部署结束】", MessageFontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.PlayerDeployedEnd, OnPlayerDeployedEnd);
            EventCenter<GameStage>.Instance.AddListener(GameStage.BattleStart, OnBattleStart);
            
            deployManager.OnDeployEnd(); // 玩家部署结束 --执行完毕-> 战斗开始
        }
        
        private void OnBattleStart()
        {
            MyConsole.Print("【战斗开始】", MessageFontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.BattleStart, OnBattleStart);
            EventCenter<GameStage>.Instance.AddListener(GameStage.BattleEnd, OnBattleEnd);
            
            CharacterManager.Instance.OnBattleStart(); // 战斗开始 --执行完毕-> 战斗结束
        }

        private void OnBattleEnd()
        {
            MyConsole.Print("【战斗结束】", MessageFontColor);
            EventCenter<GameStage>.Instance.RemoveListener(GameStage.BattleStart, OnBattleStart);
            
            CharacterManager.Instance.BattleEndAction(); // 战斗结束 --执行完毕-> ???
        }
        #endregion
    }
}