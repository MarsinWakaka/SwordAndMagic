using ConsoleSystem;
using Entity;
using GamePlaySystem.ControlCommand;
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
        private TileManager _tileManager;
        private INavigationService _navigationService;
        private CharacterManager characterManager;
        private PlayerController playerController;
        private AutoController autoController;
        
        public void Initialize(
            TileManager tileManager, INavigationService navigationService, ICommandManager commandManager)
        {
            _tileManager = tileManager;
            _navigationService = navigationService;
            characterManager = new CharacterManager();
            autoController = new GameObject("自动控制器").AddComponent<AutoController>();
            
            var aiBrain = new AIBrain(characterManager, _tileManager, _navigationService);
            autoController.Initialize(aiBrain, commandManager);
            playerController = new PlayerController(commandManager);
            // TODO 考虑使用通用的控制接口，这样就可以快速替换是玩家对战还是人机对战亦或者电子斗蛐蛐
            characterManager.Initialize(playerController, autoController);
        }
        
        private void Awake()
        {
            EventCenter<GameEvent>.Instance.AddListener<Tile>(GameEvent.OnTileCreated, TileCreatedHandle);
            EventCenter<GameEvent>.Instance.AddListener<Character>(GameEvent.OnCharacterCreated, CharacterCreateHandle);
        }
        
        private void TileCreatedHandle(Tile tile)
        {
            _tileManager.RegisterTile(tile);
        }
        
        private void CharacterCreateHandle(Character character)
        {
            characterManager.RegisterCharacter(character);
            _tileManager.InitCharacterOnTile(character);
            character.OnDeathEvent += CharacterDeadHandle;
        }
        
        private void CharacterDeadHandle(Character character)
        {
            character.OnDeathEvent -= CharacterDeadHandle;
            characterManager.UnRegisterUnit(character);
            _tileManager.RemoveCharacterOnTile(character);
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
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.BattleEnd);
        }
    }
}