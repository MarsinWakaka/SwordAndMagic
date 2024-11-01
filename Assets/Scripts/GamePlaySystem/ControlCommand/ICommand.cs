using Entity;
using GamePlaySystem.SkillSystem;
using GamePlaySystem.TileSystem.Navigation;

namespace GamePlaySystem.ControlCommand
{
    public interface ICommand
    {
    }
    
    // 如果放在构造方法里传参数的话，这些命令就不复用了，所以这里用Init方法
    // 后续会将命令对象的获取改为从CommandManager中获取，或者说是从CommandManager封装的方法中生成
    // 但最好命令生成与命令执行分离，这样可以更好的控制命令的执行过程（尤其是AI决策的时候可能会废除部分命令）
    public class FollowPathCommand : ICommand
    {
        public Character Character;
        public PathNode DestNode;
        
        /// <summary>
        /// 根据终点Node，往前遍历，直到找到起点Node(起点Node.FromNode == null)，从而生成移动路线。
        /// </summary>
        public void Init(Character character, PathNode destNode)
        {
            this.Character = character;
            this.DestNode = destNode;
        }
    }

    public class BaseSkillCommand : ICommand
    {
        public SkillSlot ChosenSkillSlot;
        public Character Caster;
        public BaseEntity[] Targets;
        public void Init(SkillSlot chosenSkillSlot, Character caster, BaseEntity[] targets)
        {
            ChosenSkillSlot = chosenSkillSlot;
            Caster = caster;
            Targets = targets;
        }
    }
}