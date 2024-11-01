using ConsoleSystem;
using Entity;
using Utility.FSM;

namespace GamePlaySystem.Controller.Player.State
{
    public class ActiveState : IState
    {
        private Character _owner;
        
        public void Initialize(Character owner)
        {
            _owner = owner;
        }
        
        public void OnEnter(object param = null)
        {
            MyConsole.Print($"[Start Turn] {_owner.characterName} - {_owner.Faction.Value} ", MessageColor.Green);
            
        }

        public void HandleMouseRightClicked()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}