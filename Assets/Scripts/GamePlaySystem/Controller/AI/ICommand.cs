using System.Collections.Generic;
using Entity;
using Entity.Unit;
using GamePlaySystem.SkillSystem;
using UnityEngine;

namespace GamePlaySystem.AISystem
{
    public interface ICommand
    {
        public bool Execute();
    }
    
    public class MoveCommand : ICommand
    {
        Character subject;
        Vector2 destination;
        public void Init(Character character, Vector2 dest)
        {
            subject = character;
            destination = dest;
        }
        
        public bool Execute()
        {
            // 移动到目标地点
            return true;
        }
    }


    public class SingleDirectSKill : ICommand
    {
        private BaseSkill _chosenSkill;
        private Character _caster;

        public void Init(BaseSkill chosenSKill, Character caster, Vector2 targetPosition)
        {
            _chosenSkill = chosenSKill;
            _caster = caster;
            // 通过TileManager获取目标
        }

        public bool Execute()
        {
            // 使用技能
            return true;
        }
    }

    public class OnSkillCommand : ICommand
    {
        private BaseSkill _chosenSkill;
        private Character _caster;
        private List<BaseEntity> _targets;
        
        public void Init(BaseSkill chosenSKill, Character caster, List<BaseEntity> targets)
        {
            _chosenSkill = chosenSKill;
            _caster = caster;
            _targets = targets;
        }
        
        public bool Execute()
        {
            // 使用技能
            return true;
        }
    }
}