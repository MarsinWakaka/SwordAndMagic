using System;
using Entity;
using GamePlaySystem.SkillSystem;
using UnityEngine;

namespace Utility.Behaviour
{
    // 对于2D游戏来说，或者说对于我这个游戏来说，角色本身是没有动画的，所以其实全局弄一个CharacterBehaviour就够了
    // 但考虑到如果拓展到3D游戏，那么每个角色都会有自己的动画，所以还是决定人均一个CharacterBehaviour
    public class CharacterBehaviour : MonoBehaviour
    {
        [Tooltip("用于帮助玩家更好的理解角色的行动")]
        [SerializeField] private GameObject actionGo;
        [SerializeField] private SpriteRenderer actionIcon;

        private Character _character;

        private void Awake()
        {
            _character = GetComponentInParent<Character>();
            _character.OnSkillChosenEnter += OnSkillChosenEnter;
            _character.OnSkillChosenExit += OnSkillChosenExit;
            actionGo.SetActive(false);
        }

        private void OnSkillChosenEnter(SkillSlot skillSlot)
        {
            actionGo.SetActive(true);
            actionIcon.sprite = skillSlot.skill.skillIcon;
        }
        
        private void OnSkillChosenExit()
        {
            actionGo.SetActive(false);
        }
    }
}