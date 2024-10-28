using ConsoleSystem;
using Entity.Tiles;
using GamePlaySystem;
using MyEventSystem;
using UnityEngine;

namespace BattleSystem
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private TileManager tileManager;
        [SerializeField] private CharacterManager characterManager;

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
    }
}