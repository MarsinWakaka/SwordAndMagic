using System.Collections.Generic;
using ConsoleSystem;
using GamePlaySystem.AISystem;
using GamePlaySystem.Controller.AI;
using GamePlaySystem.RangeDisplay;
using GamePlaySystem.TileSystem;
using GamePlaySystem.TileSystem.Navigation;
using MyEventSystem;
using UnityEngine;

namespace GamePlaySystem
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private TileManager tileManager;
        private CharacterManager characterManager;
        [SerializeField] private RangeDisplayHelper rangeDisplayHelper;
        // TODO 所以角色控制器也应该添加到这里
        private AIBrain aiBrain;
        private NavigationProvider navigationProvider;
        
        private void Awake()
        {
            characterManager = new CharacterManager();
            navigationProvider = new NavigationProvider(tileManager);
            aiBrain = new AIBrain(characterManager, tileManager, navigationProvider);
            AddListeners();
        }
        
        private void OnDestroy()
        {
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
        
        public void OnBattleStart()
        {
            characterManager.OnPlayerWin += OnBattleWin;
            characterManager.OnEnemyWin += OnBattleLose;
            characterManager.OnBattleStartAction();
        }

        private void OnBattleWin()
        {
            MyConsole.Print("[游戏结束] 玩家胜利", MessageColor.Black);
            EventCenter<GameStage>.Instance.Invoke(GameStage.BattleEnd);
        }

        private void OnBattleLose()
        {
            MyConsole.Print("[游戏结束] 敌人胜利", MessageColor.Black);
            EventCenter<GameStage>.Instance.Invoke(GameStage.BattleEnd);
        }
        
        private void ShowRangeHandle(RangeType rangeType, Vector2 position, int range)
        {
            List<PathNode> positions = null;
            switch (rangeType)
            {
                case RangeType.Movement:
                    positions = navigationProvider.GetReachablePositions(
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