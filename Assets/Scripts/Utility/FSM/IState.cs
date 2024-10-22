using System;
using Entity.Character;

namespace Utility.FSM
{
    public interface IState
    {
        public void OnEnter(Object param = null);
        public void OnUpdate();
        public void OnExit();
    }
}