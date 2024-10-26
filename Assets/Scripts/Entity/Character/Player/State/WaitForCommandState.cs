using ConsoleSystem;
using MyEventSystem;
using UnityEngine;
using Utility.FSM;

namespace Entity.Character.Player.State
{
    public class WaitForCommandState : IState
    {
        private readonly PlayerController _controller;
        public WaitForCommandState(PlayerController controller)
        {
            _controller = controller;
        }
        
        public void OnEnter(object param = null)
        {
            // TODO 思考是否需要清空角色之前选择的目标 或者 将角色的目标选择放到角色技能选择状态里
            MyConsole.Print($"[闲置状态] {_controller.CurCharacter.characterName} ", MessageColor.Green);
            
            // TODO 监听
            EventCenter<GameEvent>.Instance.AddListener<Vector2>(GameEvent.OnTileLeftClicked, GroundClickEventHandle);
            EventCenter<GameEvent>.Instance.AddListener<SkillSlot>(GameEvent.OnSkillSlotUIClicked, SkillSelectedEventHandle);
            // TODO 玩家在选择未激活对象时，也要退出这个角色的等待命令状态
        }
        
        // TODO 添加玩家鼠标右键调查角色
        // public void HandleMouseRightClicked()

        public void OnExit()
        {
            EventCenter<GameEvent>.Instance.RemoveListener<Vector2>(GameEvent.OnTileLeftClicked, GroundClickEventHandle);
            EventCenter<GameEvent>.Instance.RemoveListener<SkillSlot>(GameEvent.OnSkillSlotUIClicked, SkillSelectedEventHandle);
        }
        
        private void GroundClickEventHandle(Vector2 position)
        {
            _controller.Destination = position;
            _controller.Transition(CharacterStateType.Moving);
        }
        
        private void SkillSelectedEventHandle(SkillSlot skillSlot)
        {
            _controller.SelectedSkillSlot = skillSlot;
            var skillSlotChosen = _controller.SelectedSkillSlot;
            var skillChosen = skillSlotChosen.skill;
            if (skillSlotChosen == null)
            {
                MyConsole.Print("未选择技能", MessageColor.Red);
                // _controller.Transition(CharacterStateType.WaitForCommand);
                return;
            }
            if (skillSlotChosen.RemainCoolDown.Value > 0 || 
                _controller.CurCharacter.property.AP.Value < skillChosen.AP_Cost || 
                _controller.CurCharacter.property.SP.Value < skillChosen.SP_Cost){
                MyConsole.Print("未满足技能释放条件", MessageColor.Red);
                // _controller.Transition(CharacterStateType.WaitForCommand);
                return;
            }
            _controller.Transition(CharacterStateType.OnSkillChosen);
        }
    }
}