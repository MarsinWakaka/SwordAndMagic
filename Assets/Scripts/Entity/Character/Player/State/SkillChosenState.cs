using System;
using System.Collections.Generic;
using BattleSystem.SkillSystem;
using ConsoleSystem;
using MyEventSystem;
using UnityEngine;
using Utility.FSM;

namespace Entity.Character.Player.State
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

        public void OnEnter(object param = null)
        {
            curCharacter = _controller.CurCharacter;
            skillSlotChosen = _controller.SelectedSkillSlot;
            skillChosen = skillSlotChosen.skill;
            if (skillSlotChosen == null)
            {
                MyConsole.Print("未选择技能", MessageColor.Red);
                _controller.Transition(CharacterStateType.WaitForCommand);
                return;
            }
            if (skillSlotChosen.remainCoolDown > 0 && curCharacter.property.AP.Value < skillChosen.AP_Cost) {
                MyConsole.Print("未满足技能释放条件", MessageColor.Red);
                _controller.Transition(CharacterStateType.WaitForCommand);
                return;
            }
            MyConsole.Print($"[SkillChosenState] {curCharacter.characterName}选择了{skillChosen.skillName}", MessageColor.Green);
            EventCenter<GameEvent>.Instance.AddListener<BaseEntity>(GameEvent.OnEntityLeftClicked, OnEntityClicked);
            // TODO 显示攻击范围。
            // TODO 监听鼠标右键点击事件来取消一个目标的选择
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.OnRightMouseClick, HandleMouseRightClicked);
        }

        public void HandleMouseRightClicked()
        {
            // TODO 临时措施，后面需要与按键绑定系统结合
            if (_targets.Count > 0)
            {
                _targets.Pop();
                MyConsole.Print($"取消选择目标，进度{_targets.Count} / {skillChosen.maxTargetCount}", MessageColor.Yellow);
            }
            if (_targets.Count == 0)
            {
                _controller.Transition(CharacterStateType.WaitForCommand);
            }
        }

        public void OnExit()
        {
            _targets.Clear();
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.OnRightMouseClick, HandleMouseRightClicked);
            EventCenter<GameEvent>.Instance.RemoveListener<BaseEntity>(GameEvent.OnEntityLeftClicked, OnEntityClicked);
        }
        
        private void OnEntityClicked(BaseEntity clickedEntity)
        {
            var targetType = clickedEntity.entityType;
            var canImpactEntityType = skillChosen.canImpactEntityType;
            // var canImpactFactionType = selectedSkillSlot.skill.canImpactFactionType;
            // TODO 先判断是否在攻击范围内，再接下面判断。
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
            
            if (IsSelectFinish())
            {
                // 栈 转 数组
                _controller.Transition(CharacterStateType.OnSkillRelease, _targets.ToArray());
            }
        }
        
        private bool IsSelectFinish() => _targets.Count == skillChosen.maxTargetCount;

        private bool IsTargetInRange(BaseEntity target)
        {
            return skillSlotChosen.skill.isTargetInRange(
                curCharacter.transform.position, 
                target.transform.position);
        }
    }
}