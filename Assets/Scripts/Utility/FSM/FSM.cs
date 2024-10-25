using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entity.Character;
using UnityEngine;

namespace Utility.FSM
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FSM<TK>
    {
        private readonly Dictionary<TK, IState> _states = new();
        private IState _currentState;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void SetDefaultState(TK key)
        {
            if (_states.TryGetValue(key, out var state))
            {
                _currentState = state;
                _currentState.OnEnter();
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"State {key} not found");
            }
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Transition(TK key, object param = null)
        {
            if (_states.TryGetValue(key, out var state))
            {
                _currentState?.OnExit();
                _currentState = state;
                _currentState.OnEnter(param);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"State {key} not found");                
            }
#endif
        }

        public IState GetCurState() => _currentState;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void AddState(TK key, IState state)
        {
            if (!_states.TryAdd(key, state))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"State {key} already exists");
#endif
            }
        }
        
        public void RemoveState(TK key)
        {
            if (_states.ContainsKey(key))
                _states.Remove(key);
        }
    }
}