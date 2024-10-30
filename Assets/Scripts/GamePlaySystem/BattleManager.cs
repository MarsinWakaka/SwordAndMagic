using System.Collections.Generic;
using BattleSystem.FactionSystem;
using ConsoleSystem;
using Entity.Unit;
using GamePlaySystem.Controller;
using GamePlaySystem.Controller.AI;
using GamePlaySystem.Controller.Player;
using GamePlaySystem.FactionSystem;
using GamePlaySystem.RangeDisplay;
using GamePlaySystem.TileSystem;
using GamePlaySystem.TileSystem.Navigation;
using MyEventSystem;
using UISystem;
using UnityEngine;

namespace GamePlaySystem
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private TileManager tileManager;
        private CharacterManager characterManager;
        [SerializeField] private RangeDisplayHelper rangeDisplayHelper;
        // TODO 所以角色控制器也应该添加到这里
        private PlayerController playerController;
        private AutoController autoController;
        private NavigationService navigationService;

        // 由其它模块调用，决定是玩家对战还是人机对战，还是电子斗蛐蛐
        // public void Initialize(ICharacterController controllerA, ICharacterController controllerB)
        // {
        //     
        // }
        
        private void Awake()
        {
            navigationService = new NavigationService(tileManager);
            characterManager = new CharacterManager();
            playerController = new PlayerController();
            playerController.Initialize();
            autoController = new GameObject("自动控制器").AddComponent<AutoController>();
            autoController.Initialize(new AIBrain(characterManager, tileManager, navigationService));
            // TODO 考虑使用通用的控制接口，这样就可以快速替换是玩家对战还是人机对战亦或者电子斗蛐蛐
            characterManager.Initialize(playerController, autoController);
            AddListeners();
        }
        
        private void OnDestroy()
        {
            if (EventCenter<GameEvent>.IsInstanceNull) return;
            RemoveListeners();
        }
        
        private void AddListeners()
        {
            EventCenter<GameEvent>.Instance.AddListener<RangeType, Vector2, int> 
                (GameEvent.RangeOperation, ShowRangeHandle);
        }
        
        private void RemoveListeners()
        {
            EventCenter<GameEvent>.Instance.RemoveListener<RangeType, Vector2, int> 
                (GameEvent.RangeOperation, ShowRangeHandle);
        }
        
        public void OnBattleStartAction()
        {
            characterManager.OnFactionWin += OnFactionWin;
            UIManager.Instance.PushPanel(PanelType.BattlePanel, () => {
                characterManager.OnBattleStartAction();
            });
        }

        private void OnFactionWin(FactionType faction)
        {
            MyConsole.Print($"[游戏结束] {faction}胜利", MessageColor.Black);
            EventCenter<GameStage>.Instance.Invoke(GameStage.BattleEnd);
        }
        
        private void ShowRangeHandle(RangeType rangeType, Vector2 position, int range)
        {
            List<PathNode> positions = null;
            switch (rangeType)
            {
                case RangeType.Movement:
                    positions = navigationService.GetReachablePositions(
                        (int) position.x, (int) position.y, range);
                    break;
                case RangeType.AttackRange:
                    MyConsole.Print("【Show Attack Range】 攻击范围显示开发中", MessageColor.Red);
                    break;
            }
            rangeDisplayHelper.ShowRange(positions, rangeType);
        }
    }
}