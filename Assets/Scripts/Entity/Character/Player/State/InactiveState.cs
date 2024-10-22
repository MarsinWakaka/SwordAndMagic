using Utility.FSM;

namespace Entity.Character.Player.State
{
    public class InactiveState : IState
    {
        private readonly PlayerController _controller;
        public InactiveState(PlayerController controller)
        {
            _controller = controller;
        }


        public void OnEnter(object param = null)
        {
        }

        public void OnUpdate() { }

        public void OnExit() { }
    }
}