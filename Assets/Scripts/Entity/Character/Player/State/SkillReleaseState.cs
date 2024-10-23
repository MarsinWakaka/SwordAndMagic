using Utility.FSM;

namespace Entity.Character.Player.State
{
    public class SkillReleaseState : IState
    {        
        private readonly PlayerController _controller;
        public SkillReleaseState(PlayerController controller)
        {
            _controller = controller;
        }
        
        public void OnEnter(object param = null)
        {
            // TODO 执行技能释放逻辑，技能释放动画，施法者声音，可以和动画机事件交互
            var chosenTargets = (BaseEntity[]) param;
            // TODO 根据技能范围类型处理得到受影响的目标
            // var impactTargets = _controller.SelectedSkillSlot.skill.GetImpactTargets(_controller.CurCharacter, chosenTargets);
            if (chosenTargets != null)
            {
                var skillSlot = _controller.SelectedSkillSlot;
                var character = _controller.CurCharacter;
                
                skillSlot.remainCoolDown = skillSlot.skill.coolDown;
                character.property.AP.Value -= skillSlot.skill.AP_Cost;
                character.property.SP.Value -= skillSlot.skill.SP_Cost;
                skillSlot.OnSkillUsed();
                skillSlot.skill.Execute(_controller.CurCharacter, chosenTargets);
            }
            
            // TODO 如果有技能释放动画，可以在动画结束时调用 OnExit
            // 临时措施(目前没有技能释放动画)
            _controller.Transition(CharacterStateType.WaitForCommand);
        }

        public void HandleMouseRightClicked()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}