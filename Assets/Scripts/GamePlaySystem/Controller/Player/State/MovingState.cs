using System;
using GamePlaySystem.ControlCommand;
using GamePlaySystem.TileSystem.Navigation;
using UnityEngine;
using Utility.FSM;

namespace GamePlaySystem.Controller.Player.State
{
    public class MovingState : IState
    {
        private readonly PlayerController _controller;
        public MovingState(PlayerController controller)
        {
            _controller = controller;
        }
        
        // TODO 目的地参数从这拿
        public void OnEnter(object destNodeObj = null)
        {
            switch (destNodeObj)
            {
                case null:
                    Debug.LogError("传入的参数为空");
                    return;
                case PathNode destNode:
                {
                    var followCommand = new FollowPathCommand();
                    followCommand.Init(_controller.CurCharacter, destNode);
                    _controller.CommandManager.AddCommand(followCommand, FollowCommandComplete);
                    break;
                }
                default:
                    Debug.LogError($"传入的参数不是PathNode类型 {destNodeObj.GetType()}");
                    break;
            }

            // TODO 需要从地图管理器获取移动范围数据，使用BFS计算可到达距离和移动消耗（可以直接去旧版战斗系统移植）
            // TODO 替换直接计算曼哈顿距离
            // var character = _controller.CurCharacter;
            // var trans = character.transform;
            // int distance = Math.Abs((int)trans.position.x - (int)_controller.Destination.x) +
            //                Math.Abs((int)trans.position.y - (int)_controller.Destination.y);
            // if (character.property.RWR.Value >= distance)
            // {
            //     trans.position = _controller.Destination;
            //     character.property.RWR.Value -= distance;
            // }
            // _controller.Transition(ControllerState.WaitForCommand);
        }
        
        private void FollowCommandComplete()
        {
            _controller.Transition(ControllerState.WaitForCommand);
        }

        public void OnExit()
        {
            
        }
    }
}