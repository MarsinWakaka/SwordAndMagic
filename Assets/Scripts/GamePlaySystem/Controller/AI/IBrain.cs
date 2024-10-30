using System.Collections.Generic;
using Entity.Unit;

namespace GamePlaySystem.Controller.AI
{
    public interface IBrain
    {
        public Queue<ICommand> DoTactics(Character character);
    }
}