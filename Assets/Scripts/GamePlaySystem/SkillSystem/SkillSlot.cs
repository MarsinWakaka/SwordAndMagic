using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace GamePlaySystem.SkillSystem
{
    [Serializable] 
    public class SkillSlot
    {
        [FormerlySerializedAs("Skill")] 
        public BaseSkill skill;
        public BindableProperty<int> RemainCoolDown = new();

        public void CoolDownForOneRound() => RemainCoolDown.Value = Mathf.Max(0, RemainCoolDown.Value - 1);
    }
}