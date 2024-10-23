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
            EventCenter<GameStateEvent>.Instance.AddListener(GameStateEvent.GameStateGameResourceLoadEnd, OnGameResourceLoadEnd);
            
            // 测试加载关卡1
            GameResourceManager.Instance.OnLoadLevelResourceStart(1);// 游戏资源加载开始 --执行完毕-> 游戏资源加载结束
        }
        
        private void OnGameResourceLoadEnd()
        {
            MyConsole.Print("【游戏资源加载结束】", MessageFontColor);
            EventCenter<GameStateEvent>.Instance.RemoveListener(GameStateEvent.GameStateGameResourceLoadEnd, OnGameResourceLoadEnd);
            EventCenter<GameStateEvent>.Instance.AddListener(GameStateEvent.GameStateScenarioStart, OnScenarioStart);
            
            GameResourceManager.Instance.OnLoadLevelResourceEnd();// 游戏资源加载结束 --执行完毕-> 演出开始
        }
        
        private void OnScenarioStart()
        {
            MyConsole.Print("【演出开始】", MessageFontColor);
            EventCenter<GameStateEvent>.Instance.RemoveListener(GameStateEvent.GameStateScenarioStart, OnScenarioStart);
            EventCenter<GameStateEvent>.Instance.AddListener(GameStateEvent.GameStateScenarioEnd, OnScenarioEnd);
            
            scenarioManager.OnScenarioStart(); // 场景演出开始 --执行完毕-> 场景演出结束
        }
        
        private void OnScenarioEnd()
        {
            MyConsole.Print("【演出结束】", MessageFontColor);
            EventCenter<GameStateEvent>.Instance.RemoveListener(GameStateEvent.GameStateScenarioEnd, OnScenarioEnd);
            EventCenter<GameStateEvent>.Instance.AddListener(GameStateEvent.GameStatePlayerDeployedStart, OnPlayerDeployedStart);
            
            scenarioManager.OnScenarioEnd(); // 场景演出结束 --执行完毕-> 玩家部署开始
        }
        
        private void OnPlayerDeployedStart()
        {
            MyConsole.Print("【玩家部署开始】", MessageFontColor);
            EventCenter<GameStateEvent>.Instance.RemoveListener(GameStateEvent.GameStatePlayerDeployedStart, OnPlayerDeployedStart);
            EventCenter<GameStateEvent>.Instance.AddListener(GameStateEvent.GameStatePlayerDeployedEnd, OnPlayerDeployedEnd);
            
            deployManager.OnDeployStart(); // 玩家部署开始 --执行完毕-> 玩家部署结束
        }
        
        private void OnPlayerDeployedEnd()
        {
            MyConsole.Print("【玩家部署结束】", MessageFontColor);
            EventCenter<GameStateEvent>.Instance.RemoveListener(GameStateEvent.GameStatePlayerDeployedEnd, OnPlayerDeployedEnd);
            EventCenter<GameStateEvent>.Instance.AddListener(GameStateEvent.GameStateBattleStart, OnBattleStart);
            
            deployManager.OnDeployEnd(); // 玩家部署结束 --执行完毕-> 战斗开始
        }
        
        private void OnBattleStart()
        {
            MyConsole.Print("【战斗开始】", MessageFontColor);
            EventCenter<GameStateEvent>.Instance.RemoveListener(GameStateEvent.GameStateBattleStart, OnBattleStart);
            EventCenter<GameStateEvent>.Instance.AddListener(GameStateEvent.GameStateBattleEnd, OnBattleEnd);
            
            CharacterManager.Instance.OnBattleStart(); // 战斗开始 --执行完毕-> 战斗结束
        }

        private void OnBattleEnd()
        {
            MyConsole.Print("【战斗结束】", MessageFontColor);
            EventCenter<GameStateEvent>.Instance.RemoveListener(GameStateEvent.GameStateBattleStart, OnBattleStart);
            
            CharacterManager.Instance.BattleEndAction(); // 战斗结束 --执行完毕-> ???
        }
        #endregion
    }
}