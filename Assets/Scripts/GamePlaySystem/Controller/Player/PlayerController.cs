using Entity.Character.Player.State;
using Entity.Unit;
using Entity.Unit.State;
using GamePlaySystem.Controller.Player.State;
using UnityEngine;
using Utility.FSM;

namespace GamePlaySystem.Controller.Player
{
    public class PlayerController
    {
        private FSM<ControllerState> fsm;    // 角色控制器状态机
        private Character curCharacter;
        public Character CurCharacter => curCharacter;

        public Vector2 Destination;
        public SkillSlot SelectedSkillSlot;
        
        public void Initialize()
        {
            fsm = new FSM<ControllerState>();
            fsm.AddState(ControllerState.Inactive, new InactiveState(this));
            fsm.AddState(ControllerState.WaitForCommand, new WaitForCommandState(this));
            fsm.AddState(ControllerState.Moving, new MovingState(this));
            fsm.AddState(ControllerState.OnSkillChosen, new SkillChosenState(this));
            fsm.AddState(ControllerState.OnSkillRelease, new SkillReleaseState(this));
            
            fsm.SetDefaultState(ControllerState.Inactive);
        }
        
        public void SetCharacter(Character newCharacter)
        {
            if (curCharacter != newCharacter) {
                ResetParams();
            }
            curCharacter = newCharacter;
            if (curCharacter != null) {
                fsm.Transition(ControllerState.WaitForCommand);
            }
        }
        
        private void ResetParams()
        {
            Destination = Vector2.zero;
            SelectedSkillSlot = null;
            fsm.Transition(ControllerState.Inactive);
        }

        public void Transition(ControllerState state, object param = null)
        {
            fsm.Transition(state, param);
        }
    }
}