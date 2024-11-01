using System.Collections;
using System.Collections.Generic;
using Entity;
using GamePlaySystem.ControlCommand;
using UnityEngine;

namespace GamePlaySystem.Controller.AI
{
    public class AutoController : MonoBehaviour
    {
        private IBrain _brain;
        private ICommandManager _commandManager;
        public void Initialize(IBrain brain, ICommandManager commandManager) {
            _brain = brain;
            _commandManager = commandManager;
        }

        private readonly Queue<Character> characterWaits = new();
        
        public void AddControlQueue(Character character)
        {
            characterWaits.Enqueue(character);
            if (characterWaits.Count == 1)
            {
                StartCoroutine(DoAutoControl());
            }
        }
        
        public void AddControlQueue(IEnumerable<Character> characters)
        {
            foreach (var character in characters) {
                characterWaits.Enqueue(character);
            }
            StartCoroutine(DoAutoControl());
        }

        private readonly WaitForSeconds handleInterval = new(0.5f);
        private readonly WaitForSeconds longHandleInterval = new(2f);
        // 使用协程是因为AIBrain可能需要大量计算，来尽量分担主线程的在一帧中的压力
        private IEnumerator DoAutoControl()
        {
            while (characterWaits.Count > 0)
            {
                var curCharacter = characterWaits.Peek();
                var commandQueue = _brain.DoTactics(curCharacter);
                var curCharacterCommandCount = commandQueue.Count;
                var isCurrentCharacterAllCommandComplete = curCharacterCommandCount == 0;
                while (commandQueue.Count > 0)
                {
                    if (commandQueue.Peek() is BaseSkillCommand baseSkillCommand)
                    {
                        curCharacter.OnSkillChosenEnter(baseSkillCommand.ChosenSkillSlot);
                        yield return longHandleInterval;
                        curCharacter.OnSkillChosenExit();
                    }
                    _commandManager.ExecuteCommand(commandQueue.Dequeue(), () =>
                    {
                        isCurrentCharacterAllCommandComplete = (--curCharacterCommandCount) == 0;
                    });
                }
                // 等待当前角色的所有指令执行完毕
                while (!isCurrentCharacterAllCommandComplete) yield return handleInterval;
                characterWaits.Dequeue().SwitchEndTurnReadyState();
            }
            yield return null;
        }
    }
}