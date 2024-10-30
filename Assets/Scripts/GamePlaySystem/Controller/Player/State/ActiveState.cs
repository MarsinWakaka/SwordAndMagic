using ConsoleSystem;
using Utility.FSM;

namespace Entity.Unit.State
{
    public class ActiveState : IState
    {
        private Unit.Character _owner;
        
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