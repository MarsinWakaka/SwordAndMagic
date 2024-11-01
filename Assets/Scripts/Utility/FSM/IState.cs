using System;

namespace Utility.FSM
{
    public interface IState
    {
        public void OnEnter(Object param = null);
        public void OnExit();
    }
}