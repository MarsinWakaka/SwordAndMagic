using Entity.Character.Player.State;
using Entity.Unit.State;
using UnityEngine;
using Utility.FSM;

namespace Entity.Unit
{
    public class PlayerController
    {
        private FSM<CharacterStateType> fsm;    // 角色控制器状态机
        private Character curCharacter;
        public Character CurCharacter => curCharacter;

        public Vector2 Destination;
        public SkillSlot SelectedSkillSlot;
        
        public void InitController()
        {
            fsm = new FSM<CharacterStateType>();
            fsm.AddState(CharacterStateType.Inactive, new InactiveState(this));
            fsm.AddState(CharacterStateType.WaitForCommand, new WaitForCommandState(this));
            fsm.AddState(CharacterStateType.Moving, new MovingState(this));
            fsm.AddState(CharacterStateType.OnSkillChosen, new SkillChosenState(this));
            fsm.AddState(CharacterStateType.OnSkillRelease, new SkillReleaseState(this));
            
            fsm.SetDefaultState(CharacterStateType.Inactive);
        }
        
        private void ResetParams()
        {
            Destination = Vector2.zero;
            SelectedSkillSlot = null;
            fsm.Transition(CharacterStateType.Inactive);
        }
        
        public void SetCharacter(Character newCharacter)
        {
            if (curCharacter != newCharacter) {
                ResetParams();
            }
            curCharacter = newCharacter;
            if (curCharacter != null) {
                fsm.Transition(CharacterStateType.WaitForCommand);
            }
        }
        
        public void Transition(CharacterStateType stateType, object param = null)
        {
            fsm.Transition(stateType, param);
        }
    }
}