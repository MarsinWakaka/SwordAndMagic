using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entity.Unit;
using UnityEngine;

namespace GamePlaySystem.Controller.AI
{
    public class AutoController : MonoBehaviour
    {
        private IBrain _brain;
        public void Initialize(IBrain brain) {
            _brain = brain;
        }

        private readonly Queue<Character> characterWaitQueue = new();
        
        public void AddControlQueue(Character character)
        {
            characterWaitQueue.Enqueue(character);
            if (characterWaitQueue.Count == 1)
            {
                StartCoroutine(DoAutoControl());
            }
        }
        
        public void AddControlQueue(IEnumerable<Character> characters)
        {
            foreach (var character in characters) {
                characterWaitQueue.Enqueue(character);
            }
            StartCoroutine(DoAutoControl());
        }
        
        private IEnumerator DoAutoControl()
        {
            while (characterWaitQueue.Count > 0)
            {
                var commandQueue = _brain.DoTactics(characterWaitQueue.Peek());
                while (commandQueue.Count > 0)
                {
                    var command = commandQueue.Dequeue();
                    command.Execute();
                    yield return new WaitForSeconds(1);
                }
                characterWaitQueue.Dequeue().SwitchEndTurnReadyState();
            }
        }
    }
}