using System;
using System.Collections.Generic;
using ConsoleSystem;
using Entity;
using GamePlaySystem.RangeDisplay;
using GamePlaySystem.SkillSystem;
using GamePlaySystem.TileSystem.Navigation;
using GamePlaySystem.TileSystem.ViewField;
using MyEventSystem;
using UnityEngine;
using Utility.FSM;

namespace GamePlaySystem.Controller.Player.State
{
    public class SkillChosenState : IState
    {
        private readonly Stack<BaseEntity> _targets = new();
        private SkillSlot skillSlotChosen;  // 缓存
        private BaseSkill skillChosen;      // 缓存
        private Character curCharacter;     // 缓存
        
        private readonly PlayerController _controller;
        public SkillChosenState(PlayerController controller)
        {
            _controller = controller;
        }

        private HashSet<int> atkRangeGrids;

        public void OnEnter(object param = null)
        {
            curCharacter = _controller.CurCharacter;
            skillSlotChosen = _controller.SelectedSkillSlot;
            skillChosen = skillSlotChosen.skill;
            curCharacter.OnSkillChosenEnter?.Invoke(skillSlotChosen);  // 触发技能选择事件
            var curPos = curCharacter.transform.position;
            atkRangeGrids = ServiceLocator.Get<IViewFieldService>().GetViewFieldSets((int)curPos.x, (int)curPos.y, skillChosen.range);
            ServiceLocator.Get<IRangeDisplayService>().ShowAttackRange(atkRangeGrids);
            MyConsole.Print($"[技能选中状态] {curCharacter.CharacterName}选择了{skillChosen.skillName}", MessageColor.White);
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnSKillChosenStateEnter, skillChosen); // TODO 改为UI监听角色的状态事件，而不是通过事件中心
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.OnRightMouseClick, HandleMouseRightClicked);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.OnSkillReleaseButtonClicked, TransitionToSkillRelease);
            EventCenter<GameEvent>.Instance.AddListener<BaseEntity>(GameEvent.OnEntityLeftClicked, OnEntityClicked);
        }

        private void HandleMouseRightClicked()
        {
            // Debug.Log($"{DateTime.Now} 鼠标右键命令接受成功");
            // TODO 临时措施，后面需要与按键绑定系统结合
            // 假如从选择目标 > 0时，按下退出鼠标右键，则只取消选择目标，不退出技能选择状态
            if (_targets.Count == 0) {
                _controller.Transition(ControllerState.WaitForCommand);
                return;
            }
            
            _targets.Pop();
            EventCenter<GameEvent>.Instance.Invoke<int>(GameEvent.OnSkillTargetChoseCountChanged, _targets.Count);
            MyConsole.Print($"取消选择目标，进度{_targets.Count} / {skillChosen.maxTargetCount}", MessageColor.Yellow);
        }

        public void OnExit()
        {
            _targets.Clear();
            curCharacter.OnSkillChosenExit?.Invoke();  // 触发技能选择事件
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnSKillChosenStateExit);
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.CloseRangeOperation);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.OnRightMouseClick, HandleMouseRightClicked);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.OnSkillReleaseButtonClicked, TransitionToSkillRelease);
            EventCenter<GameEvent>.Instance.RemoveListener<BaseEntity>(GameEvent.OnEntityLeftClicked, OnEntityClicked);
        }
        
        private void OnEntityClicked(BaseEntity clickedEntity)
        {
            var targetType = clickedEntity.entityType;
            var canImpactEntityType = skillChosen.canImpactEntityType;
            // TODO 判断该阵营 是否符合 技能可影响的对象范围
            // var canImpactFactionType = selectedSkillSlot.skill.canImpactFactionType;
            if (!IsTargetInRange(clickedEntity))
            {
                MyConsole.Print($"目标超出攻击范围 {skillChosen.range}（攻击范围显示开发已加入日程 ： {DateTime.Now}）", MessageColor.Red);
                return;
            }
            // 判断目标类型是否符合技能要求
            if ((canImpactEntityType & targetType) != 0)
            {
                switch (targetType)
                {
                    case EntityType.Character:
                        _targets.Push(clickedEntity);
                        MyConsole.Print(
                            $"选择了{clickedEntity.entityType} {clickedEntity.name}，进度{_targets.Count} / {skillChosen.maxTargetCount}", MessageColor.Green);
                        break;
                    case EntityType.Item:
                        _targets.Push(clickedEntity);
                        break;
                    case EntityType.Tile:
                        _targets.Push(clickedEntity);
                        break;
                    default:
                        MyConsole.Print("未处理实体类型 " + clickedEntity.entityType, MessageColor.Red);
                        break;
                }
            }
            EventCenter<GameEvent>.Instance.Invoke<int>(GameEvent.OnSkillTargetChoseCountChanged, _targets.Count);
            if (IsSelectFinish()) TransitionToSkillRelease();
        }
        
        private bool IsSelectFinish() => _targets.Count == skillChosen.maxTargetCount;

        private bool IsTargetInRange(BaseEntity target)
        {
            var targetPos = target.transform.position;
            var key = NavigationService.GetIndexKey((int)targetPos.x, (int)targetPos.y);
            return atkRangeGrids.Contains(key);
            // return skillSlotChosen.skill.isTargetInATKRange(
            //     curCharacter.transform.position, 
            //     target.transform.position);
        }
        
        private void TransitionToSkillRelease()
        {
            _controller.Transition(ControllerState.OnSkillRelease, _targets.ToArray());
        }
    }
}