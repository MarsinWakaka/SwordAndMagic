using System;
using System.Collections.Generic;
using BattleSystem.FactionSystem;
using ConsoleSystem;
using Entity.Character;
using Entity.Character.Player;
using MyEventSystem;
using UISystem;
using Utility.Singleton;

namespace BattleSystem
{
    /// <summary>
    /// 用于管理角色的轮询行动
    /// </summary>
    public class CharacterManager : SingletonMono<CharacterManager>
    {
        private readonly List<Character> units = new();
        private readonly Dictionary<FactionType, List<Character>> factionUnits = new();
        private int enemyDeadCount, playerDeadCount;    // 引入此变量减少CPU计算量，缺点需要维护两个变量

        private readonly PlayerController playerController = new();
        
        protected override void Awake()
        {
            base.Awake();
            foreach (FactionType faction in Enum.GetValues(typeof(FactionType)))
                factionUnits.Add(faction, new List<Character>());
            playerController.InitController();
            EventCenter<GameEvent>.Instance.AddListener<Character>(GameEvent.OnCharacterCreated, RegisterCharacter);
            EventCenter<GameEvent>.Instance.AddListener<Character>(GameEvent.OnCharacterSlotUIClicked, CharacterSelectedHandle);
        }

        #region 角色轮询控制
        
        private readonly List<Character> activeCharacters = new();  // 当前处于行动状态的角色列表
        private int unitIndexLc, unitIndexRo, readyToEndCount; // 左闭右开 [C, O)
        
        // 下一批角色行动
        private void NextBatchUnit()
        {
            readyToEndCount = 0;
            activeCharacters.Clear();
            
            // 记录当前行动单位的阵营，接下来连续相同阵营的一批可以开始行动
            unitIndexLc = unitIndexRo;
            var curUnit = units[unitIndexLc];
            var curFaction = curUnit.Faction.Value;
            activeCharacters.Add(curUnit);
            
            // 相同阵营的单位且行动顺序连续的可以一起行动
            while (true)
            {
                unitIndexRo = ToNextIndex(unitIndexRo);
                curUnit = units[unitIndexRo];
                if (curUnit.IsDead) continue;   // 角色死亡跳过
                if (curUnit.Faction.Value != curFaction) break;
                activeCharacters.Add(curUnit);
            }
            // 请确保角色开始回合时，此循环执行的过程(这一帧内)中修改此循环的值
            foreach (var activeUnit in activeCharacters)
            {
                UnitStartTurn(activeUnit);
            }
            // 如果可以行动的友方角色，切换SelectCharacterUI到第一个行动的角色
            if (activeCharacters[0].Faction.Value == FactionType.Player)
            {
                CharacterSelectedHandle(activeCharacters[0]);
            }
            
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.UpdateUIOfActionUnitOrder, GetCharacterOrder());
        }

        /// <summary>
        /// 单位切换至开始回合状态
        /// </summary>
        private void UnitStartTurn(Character character)
        {
            MyConsole.Print($"[新单位 {character.characterName} 开始回合] 准备结束 - {readyToEndCount} / {activeCharacters.Count}", MessageColor.Yellow);
            // TODO 思考是否需要判断角色死亡眩晕等失能状态
            character.ReadyToEndEvent += UnitReadyToEndHandle;
            character.CancelReadyToEndEvent += CancelUnitReadyToEndHandle;
            // 角色开始行动时的回复逻辑
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
            {
                // 等待0.2s 下一批单位行动
                // 因为敌人开始回合时 NextBatchUnit调用，但是在UnitStartTurn里敌人如果在0帧中完成了行动，导致调用UnityReadyToEndHandle
                // 然后由于清空了activeCharacters的值，导致在遍历activeCharacters时foreach发生循环错误
                Invoke(nameof(BatchUnitEndTurn), 0.2f);
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
                if (isPlayerWin)
                    MyConsole.Print("[游戏结束] 玩家胜利", MessageColor.Black);
                else
                    MyConsole.Print("[游戏结束] 敌人胜利", MessageColor.Black);
                EventCenter<GameStateEvent>.Instance.Invoke(GameStateEvent.GameStateBattleEnd);
                return true;
            }

            return false;
        }

        private int ToNextIndex(int index) => (index + 1) % units.Count;

        #endregion
        
        #region 角色选中处理

        private Character lastSelectedCharacter;
        
        /// <summary>
        /// 设置一个角色为选中状态，不涉及UI的更新
        /// </summary>
        private void CharacterSelectedHandle(Character newSelectedCharacter)
        {
            // 如果当前角色处于等待命令状态 OR 其它进一步状态，则退出
            if (lastSelectedCharacter != null && lastSelectedCharacter.IsOnTurn) 
                playerController.SetCharacter(null);    // 此为退出操作
            
            // TODO 判断角色是否满足行动条件，例如眩晕、死亡等
            if (newSelectedCharacter.IsOnTurn)
            {
                playerController.SetCharacter(newSelectedCharacter);
            }
            // 通知UI显示选中单位的信息,谨防事件触发循环
            // MyConsole.Print("[事件触发]" + GameEvent.UpdateUIOfSelectedCharacter, MessageColor.Black);
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.UpdateUIOfSelectedCharacter, newSelectedCharacter);
            lastSelectedCharacter = newSelectedCharacter;
        }

        #endregion

        #region 角色生成 与 死亡
        
        // TODO 对于角色顺序索引的影响是什么？可能会导致索引越界怎么解决？ -> 通过为角色添加死亡标记解决
        // 解决方案，死亡后将其标记为Dead，不再参与行动，但是不从列表中移除，这样不会影响索引，同时遍历等操作时等遇到Dead时跳过即可
        
        /// <summary>
        /// 角色单元生成时自动调用，不需要其它模块调用
        /// </summary>
        private void RegisterCharacter(Character character)
        {
            character.OnDeathEvent += UnRegisterUnit;
            units.Add(character);
            factionUnits[character.Faction.Value].Add(character);
        }

        /// <summary>
        /// 角色单元死后自动调用，不需要其它模块调用
        /// </summary>
        /// <param name="character"></param>
        private void UnRegisterUnit(Character character)
        {
            character.OnDeathEvent -= UnRegisterUnit;
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
        
        
        // TODO 这部分代码应该移到BattleManager中(暂未添加BattleManager)
        #region TODO
        
        public void OnBattleStart()
        {
            units.Sort();
            UIManager.Instance.PushPanel(PanelType.BattlePanel);
            NextBatchUnit();
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.UpdateUIOfPlayerParty, factionUnits[FactionType.Player].ToArray());
        }

        public void BattleEndAction()
        {
            // TODO 通知UI显示战斗结束
            MyConsole.Print("[战斗结束]", MessageColor.Black);
            UIManager.Instance.PushPanel(PanelType.BattleEndPanel);
        }

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