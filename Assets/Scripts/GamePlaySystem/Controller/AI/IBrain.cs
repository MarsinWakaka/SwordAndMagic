using System.Collections.Generic;
using Entity;
using GamePlaySystem.ControlCommand;

namespace GamePlaySystem.Controller.AI
{
    public interface IBrain
    {
        public Queue<ICommand> DoTactics(Character character);
    }
}