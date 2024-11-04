using System.Collections;
using Entity;
using GamePlaySystem.SkillSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utility.Behaviour
{
    // 对于2D游戏来说，或者说对于我这个游戏来说，角色本身是没有动画的，所以其实全局弄一个CharacterBehaviour就够了
    // 但考虑到如果拓展到3D游戏，那么每个角色都会有自己的动画，所以还是决定人均一个CharacterBehaviour
    public class CharacterBehaviour : MonoBehaviour
    {
        [FormerlySerializedAs("characterIcon")]
        [Tooltip("用于帮助玩家更好的理解角色的行动")]
        [SerializeField] private SpriteRenderer characterSr;
        [SerializeField] private GameObject actionGo;
        [SerializeField] private SpriteRenderer actionIcon;

        private Character _character;

        private void Awake()
        {
            characterSr = transform.parent.GetComponentInChildren<SpriteRenderer>();
            _character = GetComponentInParent<Character>();
            _character.OnSkillChosenEnter += OnSkillChosenEnter;
            _character.OnSkillChosenExit += OnSkillChosenExit;
            _character.OnTakeDamage += OnTakeDamage;
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

        private void OnTakeDamage(DamageType type, int damage)
        {
            StartCoroutine(OnTakeDamageFX());
        }
        
        private IEnumerator OnTakeDamageFX()
        {
            // 闪烁
            var color = characterSr.color;
            for (int i = 0; i < 3; i++)
            {
                characterSr.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                characterSr.color = color;
                yield return new WaitForSeconds(0.1f);
            }
        } 
    }
}