using Entity.Unit;
using Utility.FSM;

namespace GamePlaySystem.Controller.Player.State
{
    public class InactiveState : IState
    {
        private readonly PlayerController _controller;
        private Character _curCharacter;
        public InactiveState(PlayerController controller)
        {
            _controller = controller;
        }

        public void OnEnter(object param = null)
        {
            // _curCharacter = _controller.CurCharacter;
            // if (_curCharacter != null)
            // {
            //     _curCharacter.SwitchEndTurnReadyState();
            // }
        }

        public void HandleMouseRightClicked() { }

        public void OnExit() { }
    }
}