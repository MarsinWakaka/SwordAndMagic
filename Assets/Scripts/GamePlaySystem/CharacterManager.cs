using System;
using System.Collections.Generic;
using CameraSystem;
using ConsoleSystem;
using Entity;
using GamePlaySystem.Controller.AI;
using GamePlaySystem.Controller.Player;
using GamePlaySystem.FactionSystem;
using MyEventSystem;

namespace GamePlaySystem
{
    /// <summary>
    /// 用于管理角色的轮询行动
    /// </summary>
    public class CharacterManager // : SingletonMono<CharacterManager>
    {
        private readonly List<Character> units = new();
        private readonly Dictionary<FactionType, List<Character>> factionUnits = new();
        private int enemyDeadCount, playerDeadCount;    // 引入此变量减少计算量，缺点需要维护两个变量
        public Action<FactionType> OnFactionWin;
        public List<Character> GetUnitsByFaction(FactionType faction) => factionUnits[faction];
        
        // public Action<FactionType, List<Character>> OnCharactersAction;
        
        private PlayerController _playerController;
        private AutoController _autoController;
        
        /// <summary>
        /// 需要两个控制器
        /// </summary>
        public void Initialize(PlayerController playerController, AutoController autoController)
        {
            this._playerController = playerController;
            this._autoController = autoController;
        }

        public CharacterManager()
        {
            foreach (FactionType faction in Enum.GetValues(typeof(FactionType)))
                factionUnits.Add(faction, new List<Character>());
            EventCenter<GameEvent>.Instance.AddListener<Character>(GameEvent.OnCharacterSlotUIClicked, CharacterSelectedHandle);
        }

        public void OnBattleStartAction()
        {
            units.Sort();
            NextBatchUnit();
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.UpdateUIOfPlayerParty, factionUnits[FactionType.Player].ToArray());
        }

        
        #region 角色轮询控制
        private readonly List<Character> activeCharacters = new();  // 当前处于行动状态的角色列表
        private int unitIndexLc, unitIndexRo, readyToEndCount; // 左闭右开 [C, O)
        
        /// <summary>
        ///  下一批角色行动
        /// </summary>
        private void NextBatchUnit()
        {
            TryHandleGameOver();
            
            readyToEndCount = 0;
            activeCharacters.Clear();
            
            // 记录当前行动单位的阵营，接下来连续相同阵营的一批可以开始行动
            unitIndexLc = unitIndexRo;
            var curUnit = units[unitIndexLc];
            var curFaction = curUnit.Faction.Value;
            activeCharacters.Add(curUnit);
            
            // 相同阵营的单位且行动顺序连续的可以一起行动
            while (activeCharacters.Count <= OrderSlotCount) // 这边最高同时行动行动指示条槽位个数的单位
            {
                unitIndexRo = ToNextIndex(unitIndexRo);
                curUnit = units[unitIndexRo];
                if (curUnit.IsDead) continue;   // 角色死亡跳过
                if (curUnit.Faction.Value != curFaction) break;
                activeCharacters.Add(curUnit);
            }
            // ## new
            // OnCharactersAction?.Invoke(curFaction, activeCharacters);
            // ##
            foreach (var activeUnit in activeCharacters)
            {
                UnitStartTurn(activeUnit);
            }
            // 如果可以行动的友方角色，切换SelectCharacterUI到第一个行动的角色
            if (activeCharacters[0].Faction.Value == FactionType.Player) {
                CharacterSelectedHandle(activeCharacters[0]);
            } else {
                _playerController.SetCharacter(null);
                _autoController.AddControlQueue(activeCharacters);
            }
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.UpdateUIOfActionUnitOrder, GetCharacterOrder());
        }

        /// <summary>
        /// 单位切换至开始回合状态
        /// </summary>
        private void UnitStartTurn(Character character)
        {
            MyConsole.Print($"[新单位 {character.CharacterName} 开始回合] 准备结束 - {readyToEndCount} / {activeCharacters.Count}", MessageColor.Yellow);
            // TODO 思考是否需要判断角色死亡眩晕等失能状态
            character.ReadyToEndEvent += UnitReadyToEndHandle;
            character.CancelReadyToEndEvent += CancelUnitReadyToEndHandle;
            character.StartTurn();
        }
        
        /// <summary>
        /// 单位进去 准备结束状态
        /// </summary>
        private void UnitReadyToEndHandle()
        {
            readyToEndCount++;
            MyConsole.Print($"[单位准备结束] 准备结束 -  {readyToEndCount} / {activeCharacters.Count}", MessageColor.Yellow);
            if (activeCharacters.Count == readyToEndCount)
                BatchUnitEndTurn();
            else if (activeCharacters[readyToEndCount].Faction.Value == FactionType.Player)
            {
                foreach (var character in activeCharacters)
                {
                    if (character.IsDead) continue;
                    if (character.IsReadyToEndTurn) continue;
                    CharacterSelectedHandle(character); // 选中下一个未准备结束的角色
                    break;
                }
            }
        }
        
        /// <summary>
        /// 单位取消准备结束状态
        /// </summary>
        private void CancelUnitReadyToEndHandle()
        {
            readyToEndCount--;
            MyConsole.Print($"[单位取消准备结束] 准备结束角色个数 - {readyToEndCount} / {activeCharacters.Count}", MessageColor.Yellow);
        }
        
        /// <summary>
        /// 单位结束行动
        /// </summary>
        private void BatchUnitEndTurn()
        {
            foreach (var character in activeCharacters)
            {  
                character.EndTurn();
                character.ReadyToEndEvent -= UnitReadyToEndHandle;
                character.CancelReadyToEndEvent -= CancelUnitReadyToEndHandle;
            }
            // 清空当前激活的角色列表
            activeCharacters.Clear();
            MyConsole.Print($"[此批角色行动结束]", MessageColor.Orange);
            
            NextBatchUnit();
        }

        /// <summary>
        /// 返回游戏是否结束，如果结束自动调用OnBattleEnd
        /// </summary>
        private bool TryHandleGameOver()
        {
            bool isPlayerWin = enemyDeadCount == factionUnits[FactionType.Enemy].Count;
            bool isEnemyWin = playerDeadCount == factionUnits[FactionType.Player].Count;
            if (isPlayerWin || isEnemyWin)
            {
                OnFactionWin?.Invoke(isPlayerWin ? FactionType.Player : FactionType.Enemy);
                return true;
            }
            return false;
        }

        private int ToNextIndex(int index) => (index + 1) % units.Count;
        #endregion
        
        
        #region 角色选中处理
        private Character playerLastSelectedCharacter;
        
        /// <summary>
        /// 设置一个角色为选中状态，不涉及UI的更新
        /// </summary>
        private void CharacterSelectedHandle(Character newSelectedCharacter)
        {
            if (newSelectedCharacter.Faction.Value == FactionType.Player)
            {
                // 如果当前角色处于等待命令状态 OR 其它进一步状态，则退出
                if (playerLastSelectedCharacter != null && playerLastSelectedCharacter.IsOnTurn) 
                    _playerController.SetCharacter(null);    // 此为退出操作
                // TODO 判断角色是否满足行动条件，例如眩晕、死亡等
                if (newSelectedCharacter.IsOnTurn) {
                    _playerController.SetCharacter(newSelectedCharacter);
                }
                EventCenter<GameEvent>.Instance.Invoke(GameEvent.UpdateUIOfSelectedCharacter, newSelectedCharacter);
                playerLastSelectedCharacter = newSelectedCharacter;
            }
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.CameraMoveToPosition, newSelectedCharacter.transform.position);
        }
        #endregion

        
        #region 角色生成 与 死亡
        // 角色死亡对于角色顺序索引的影响是什么？可能会导致索引越界怎么解决？ -> 通过为角色添加死亡标记解决
        // 解决方案，死亡后将其标记为Dead，不再参与行动，但是不从列表中移除，这样不会影响索引，同时遍历等操作时等遇到Dead时跳过即可
        
        /// <summary>
        /// 角色单元生成时自动调用，不需要其它模块调用
        /// </summary>
        public void RegisterCharacter(Character character)
        {
            units.Add(character);
            factionUnits[character.Faction.Value].Add(character);
        }

        /// <summary>
        /// 角色单元死后自动调用，不需要其它模块调用
        /// </summary>
        /// <param name="character"></param>
        public void UnRegisterUnit(Character character)
        {
            // TODO 检查游戏是否结束
            if (character.Faction.Value == FactionType.Player)
                playerDeadCount++;
            else if (character.Faction.Value == FactionType.Enemy)
                enemyDeadCount++;
            if (TryHandleGameOver()) {
                return;
            }
            
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.UpdateUIOfActionUnitOrder, GetCharacterOrder());
            if (character.Faction.Value == FactionType.Player)
                EventCenter<GameEvent>.Instance.Invoke(GameEvent.UpdateUIOfPlayerParty,  factionUnits[FactionType.Player].ToArray());
            // TODO 如果是选中角色死亡，更新SelectCharacterUI
        }
        #endregion
        
        
        // TODO 这部分代码应该移到BattleManager中,等待物品管理器的实现ing...
        #region TODO
        private const int OrderSlotCount = 10;
        readonly Character[] orderSlots = new Character[OrderSlotCount];
        private Character[] GetCharacterOrder()
        {
            if (units.Count == 0) return null;
            // 只显示之后的10个角色
            int tempIndex = unitIndexLc;
            for(int i = 0; i < OrderSlotCount; i++)
            {
                Character curUnit;
                do {
                    curUnit = units[tempIndex];
                    tempIndex = ToNextIndex(tempIndex);
                } while (curUnit.IsDead);
                orderSlots[i] = curUnit;
            }
            return orderSlots;
        }
        #endregion
    }
}