using Entity;
using GamePlaySystem.ControlCommand;
using GamePlaySystem.Controller.Player.State;
using GamePlaySystem.SkillSystem;
using Utility.FSM;

namespace GamePlaySystem.Controller.Player
{
    public class PlayerController
    {
        private readonly FSM<ControllerState> fsm;    // 角色控制器状态机
        public SkillSlot SelectedSkillSlot;
        public ICommandManager CommandManager { get; }
        public Character CurCharacter { get; private set; }

        public PlayerController(ICommandManager commandManager)
        {
            this.CommandManager = commandManager;
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
            if (CurCharacter != newCharacter) {
                ResetParams();
            }
            CurCharacter = newCharacter;
            if (CurCharacter != null) {
                fsm.Transition(ControllerState.WaitForCommand);
            }
        }
        
        private void ResetParams()
        {
            SelectedSkillSlot = null;
            fsm.Transition(ControllerState.Inactive);
        }

        public void Transition(ControllerState state, object param = null)
        {
            fsm.Transition(state, param);
        }
    }
}