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
                    _controller.CommandManager.ExecuteCommand(followCommand, FollowCommandComplete);
                    break;
                }
                default:
                    Debug.LogError($"传入的参数不是PathNode类型 {destNodeObj.GetType()}");
                    break;
            }
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