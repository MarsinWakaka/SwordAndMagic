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
        
        public void HandleMouseRightClicked()
        {
            
        }

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
            _controller.Transition(CharacterStateType.OnSkillChosen);
        }
        
        // private void OnEntityLeftClicked(BaseEntity baseEntity)
        // {
        //     
        // }
    }
}