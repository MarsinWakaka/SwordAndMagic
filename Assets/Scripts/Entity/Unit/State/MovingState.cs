

using System;
using Entity.Unit;
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
            // TODO 需要从地图管理器获取移动范围数据，使用BFS计算可到达距离和移动消耗（可以直接去旧版战斗系统移植）
            // TODO 替换直接计算曼哈顿距离
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

        public void HandleMouseRightClicked()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}