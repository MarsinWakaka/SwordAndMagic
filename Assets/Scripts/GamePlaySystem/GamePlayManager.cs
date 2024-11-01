using BattleSystem.ScenarioSystem;
using Configuration;
using ConsoleSystem;
using Entity;
using GamePlaySystem.ControlCommand;
using GamePlaySystem.DeploySystem;
using GamePlaySystem.LevelData;
using GamePlaySystem.RangeDisplay;
using GamePlaySystem.TileSystem;
using GamePlaySystem.TileSystem.Navigation;
using GamePlaySystem.TileSystem.ViewField;
using MyEventSystem;
using UISystem;
using UnityEngine;

namespace GamePlaySystem
{
    public class GamePlayManager : MonoBehaviour
    {
        [Header("游戏玩法需要注册的Mono服务")]
        [SerializeField] private TileManager tileManager;
        [SerializeField] private RangeDisplayService rangeDisplayService;
        
        private EntityFactoryImpl entityFactory;
        private LevelDataManager levelDataManager;
        [SerializeField] private ScenarioManager scenarioManager;
        [SerializeField] private DeployManager deployManager;
        [SerializeField] private BattleManager battleManager;
        
        private const MessageColor FontColor = MessageColor.Magenta;
        
        private void Awake()
        {
            var config = ServiceLocator.Get<IConfigService>().ConfigData;
            // TODO 解决部署资源获取问题，从而将服务的注册与卸载代码移到场景切换类中
            entityFactory = new EntityFactoryImpl(config.characterPrefabPath, config.tilePrefabPath);
            var navigationService = new NavigationService(tileManager);
            var commandManager = new GameObject("CommandManager").AddComponent<CommandManager>();
            commandManager.transform.SetParent(transform);
            
            ServiceLocator.Register<TileManager>(tileManager);
            ServiceLocator.Register<IEntityFactory>(entityFactory);
            ServiceLocator.Register<IRangeDisplayService>(rangeDisplayService);
            ServiceLocator.Register<INavigationService>(navigationService);
            ServiceLocator.Register<IViewFieldService>(new SimpleViewField(tileManager));
            ServiceLocator.Register<ICommandManager>(commandManager);
            levelDataManager = new LevelDataManager(new LevelDataProcessor(config.levelDataParser));
            battleManager.Initialize(tileManager, navigationService, commandManager);
            // TODO 读取资源后往DeployManager里面传递
            // deployManager.Initialize(tileManager, );
            // 战斗阶段的入口
            EventCenter<GameEvent>.Instance.AddListener<int>(GameEvent.GameResourceLoadStart, OnGameResourceLoadStart);
        }
        
        private void OnDestroy()
        {
            if (EventCenter<GameEvent>.IsInstanceNull) return;
            ServiceLocator.Unregister<TileManager>();
            ServiceLocator.Unregister<IEntityFactory>();
            ServiceLocator.Unregister<IRangeDisplayService>();
            ServiceLocator.Unregister<INavigationService>();
            ServiceLocator.Unregister<IViewFieldService>();
            ServiceLocator.Unregister<ICommandManager>();
        }

        private void OnGameResourceLoadStart(int sceneIndex)
        {
            MyConsole.Print("【游戏资源加载开始】", FontColor);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.GameResourceLoadEnd, OnGameResourceLoadEnd);
            entityFactory.LoadEntityData(() => {
                // 测试加载关卡1
                levelDataManager.OnLoadLevelResourceStart(sceneIndex);// 游戏资源加载开始 --执行完毕-> 游戏资源加载结束
            });
        }
        
        private void OnGameResourceLoadEnd()
        {
            MyConsole.Print("【游戏资源加载结束】", FontColor);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.GameResourceLoadEnd, OnGameResourceLoadEnd);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.ScenarioStart, OnScenarioStart);
            
            levelDataManager.OnLoadLevelResourceEnd();// 游戏资源加载结束 --执行完毕-> 演出开始
        }
        
        private void OnScenarioStart()
        {
            MyConsole.Print("【演出开始】", FontColor);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.ScenarioStart, OnScenarioStart);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.ScenarioEnd, OnScenarioEnd);
            
            scenarioManager.OnScenarioStart(); // 场景演出开始 --执行完毕-> 场景演出结束
        }
        
        private void OnScenarioEnd()
        {
            MyConsole.Print("【演出结束】", FontColor);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.ScenarioEnd, OnScenarioEnd);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.PlayerDeployedStart, OnPlayerDeployedStart);
            
            scenarioManager.OnScenarioEnd(); // 场景演出结束 --执行完毕-> 玩家部署开始
        }
        
        private void OnPlayerDeployedStart()
        {
            MyConsole.Print("【玩家部署开始】", FontColor);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.PlayerDeployedStart, OnPlayerDeployedStart);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.PlayerDeployedEnd, OnPlayerDeployedEnd);
            
            deployManager.OnDeployStart(); // 玩家部署开始 --执行完毕-> 玩家部署结束
        }
        
        private void OnPlayerDeployedEnd()
        {
            MyConsole.Print("【玩家部署结束】", FontColor);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.PlayerDeployedEnd, OnPlayerDeployedEnd);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.BattleStart, OnBattleStart);
            deployManager.OnDeployEnd(); // 玩家部署结束 --执行完毕-> 战斗开始
        }
        
        private void OnBattleStart()
        {
            MyConsole.Print("【战斗开始】", FontColor);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.BattleStart, OnBattleStart);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.BattleEnd, OnBattleEnd);
            
            battleManager.OnBattleStartAction(); // 战斗开始 --执行完毕-> 战斗结束
        }

        private void OnBattleEnd()
        {
            MyConsole.Print("【战斗结束】", FontColor);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.BattleEnd, OnBattleEnd);
            
            GamePlayEnd();
        }

        private void GamePlayEnd()
        {
            MyConsole.Print("[游戏结束]", MessageColor.Black);
            UIManager.Instance.PushPanel(PanelType.BattleEndPanel, null);
        }
    }
}