

using System;
using Utility.FSM;

namespace Entity.Character.Player.State
{
    public class MovingState : IState
    {
        private readonly PlayerController _controller;
        public MovingState(PlayerController controller)
        {
            _controller = controller;
        }
        
        // TODO 目的地参数从这拿
        public void OnEnter(object param = null)
        {
            // TODO Do Something when character is moving
            // 曼哈顿距离
            var character = _controller.CurCharacter;
            var trans = character.transform;
            int distance = Math.Abs((int)trans.position.x - (int)_controller.Destination.x) +
                           Math.Abs((int)trans.position.y - (int)_controller.Destination.y);
            if (character.property.RWR.Value >= distance)
            {
                trans.position = _controller.Destination;
                character.property.RWR.Value -= distance;
            }
            _controller.Transition(CharacterStateType.WaitForCommand);
        }

        public void OnUpdate()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}