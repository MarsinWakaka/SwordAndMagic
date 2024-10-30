using System;
using Entity;
using Entity.Character;
using Entity.Unit;
using GamePlaySystem.FactionSystem;
using UnityEngine;

namespace BattleSystem.FactionSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FactionIndicator : MonoBehaviour
    {
        [Header("需要父对象为Character\n")]
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (transform.parent.TryGetComponent(out Character character))
            {
                character.Faction.OnValueChanged += SetFactionColor;
            }
#if UNITY_EDITOR
            else
            {
                print("FactionIndicator: 父对象不是Character");
            }
#endif
        }

        private void SetFactionColor(FactionType faction)
        {
            _spriteRenderer.color = FactionManager.GetFactionColor(faction);
        }
    }
}