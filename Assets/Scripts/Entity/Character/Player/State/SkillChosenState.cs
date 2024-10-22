using System.Collections.Generic;
using ConsoleSystem;
using EventSystem;
using Utility.FSM;

namespace Entity.Character.Player.State
{
    public class SkillChosenState : IState
    {
        private readonly Stack<BaseEntity> _targets = new();
        private SkillSlot selectedSkillSlot;    // 缓存
        
        private readonly PlayerController _controller;
        public SkillChosenState(PlayerController controller)
        {
            _controller = controller;
        }

        public void OnEnter(object param = null)
        {
            selectedSkillSlot = _controller.SelectedSkillSlot;
            if (selectedSkillSlot == null)
            {
                MyConsole.Print("未选择技能", MessageColor.Red);
                _controller.Transition(CharacterStateType.WaitForCommand);
                return;
            }
            if (selectedSkillSlot.remainCoolDown > 0 && _controller.CurCharacter.property.AP.Value < selectedSkillSlot.skill.AP_Cost) {
                MyConsole.Print("未满足技能释放条件", MessageColor.Red);
                _controller.Transition(CharacterStateType.WaitForCommand);
                return;
            }
            var skill = selectedSkillSlot.skill;
            MyConsole.Print($"[SkillChosenState] {_controller.CurCharacter.characterName}选择了{skill.skillName}", MessageColor.Green);
            EventCenter<GameEvent>.Instance.AddListener<BaseEntity>(GameEvent.OnEntityLeftClicked, OnEntityClicked);
            // TODO 监听鼠标右键点击事件
        }

        public void OnUpdate() { }

        public void OnExit()
        {
            _targets.Clear();
            // TODO 执行技能释放逻辑
            EventCenter<GameEvent>.Instance.RemoveListener<BaseEntity>(GameEvent.OnEntityLeftClicked, OnEntityClicked);
        }
        
        private void OnEntityClicked(BaseEntity clickedEntity)
        {
            var targetType = clickedEntity.entityType;
            var canImpactEntityType = selectedSkillSlot.skill.canImpactEntityType;
            // var canImpactFactionType = selectedSkillSlot.skill.canImpactFactionType;
            if ((canImpactEntityType & targetType) != 0)
            {
                switch (targetType)
                {
                    case EntityType.Character:
                        _targets.Push(clickedEntity);
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
        
        private bool IsSelectFinish() => _targets.Count == selectedSkillSlot.skill.maxTargetCount;
    }
}