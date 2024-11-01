using System.Collections.Generic;
using ConsoleSystem;
using Entity;
using GamePlaySystem.RangeDisplay;
using GamePlaySystem.SkillSystem;
using GamePlaySystem.TileSystem.Navigation;
using MyEventSystem;
using UnityEngine;
using Utility.FSM;

namespace GamePlaySystem.Controller.Player.State
{
    public class WaitForCommandState : IState
    {
        private readonly PlayerController _controller;
        private Character _curCharacter;
        public WaitForCommandState(PlayerController controller)
        {
            _controller = controller;
        }
        
        private Dictionary<int, PathNode> _pathNodeTmp;
        
        public void OnEnter(object param = null)
        {
            _curCharacter = _controller.CurCharacter;
            // TODO 思考是否需要清空角色之前选择的目标 或者 将角色的目标选择放到角色技能选择状态里
            MyConsole.Print($"[闲置状态] {_curCharacter.CharacterName} ", MessageColor.Green);
            var navigationService = ServiceLocator.Get<INavigationService>();
            var rangeDisplayService = ServiceLocator.Get<IRangeDisplayService>();
            var pos = _curCharacter.transform.position;
            var rwr = _curCharacter.Property.RWR.Value;
            _pathNodeTmp = navigationService.GetReachablePositionDict((int)pos.x, (int)pos.y, rwr);
            rangeDisplayService.ShowMoveRange(_pathNodeTmp, rwr);
            EventCenter<GameEvent>.Instance.AddListener<BaseEntity>(GameEvent.OnEntityLeftClicked, EntityClickedHandle);
            EventCenter<GameEvent>.Instance.AddListener<SkillSlot>(GameEvent.OnSkillSlotUIClicked, SkillSelectedEventHandle);
            // TODO 玩家在选择未激活对象时，也要退出这个角色的等待命令状态
        }
        
        // TODO 添加玩家鼠标右键调查角色
        // public void HandleMouseRightClicked()

        public void OnExit()
        {
            EventCenter<GameEvent>.Instance.RemoveListener<BaseEntity>(GameEvent.OnEntityLeftClicked, EntityClickedHandle);
            EventCenter<GameEvent>.Instance.RemoveListener<SkillSlot>(GameEvent.OnSkillSlotUIClicked, SkillSelectedEventHandle);
        }
        
        private void EntityClickedHandle(BaseEntity entity)
        {
            if (entity is Tile tile)
            {
                var position = tile.transform.position;
                var destKey = NavigationService.GetIndexKey((int)position.x, (int)position.y);
                if (_pathNodeTmp.TryGetValue(destKey, out var destNode))
                {
                    _controller.Transition(ControllerState.Moving, destNode);
                }
                else
                {
                    MyConsole.Print("无法到达的位置", MessageColor.Red);
                }
            }
        }
        
        private void SkillSelectedEventHandle(SkillSlot skillSlot)
        {
            _controller.SelectedSkillSlot = skillSlot;
            var skillSlotChosen = _controller.SelectedSkillSlot;
            var skillChosen = skillSlotChosen.skill;
            if (skillSlotChosen == null)
            {
                Debug.LogError("选中的技能槽为空");
                return;
            }
            if (skillSlotChosen.RemainCoolDown.Value > 0 || 
                _controller.CurCharacter.Property.AP.Value < skillChosen.AP_Cost || 
                _controller.CurCharacter.Property.SP.Value < skillChosen.SP_Cost){
                MyConsole.Print("未满足技能释放条件", MessageColor.Red);
                // _controller.Transition(ControllerState.WaitForCommand);
                return;
            }
            _controller.Transition(ControllerState.OnSkillChosen);
        }
    }
}