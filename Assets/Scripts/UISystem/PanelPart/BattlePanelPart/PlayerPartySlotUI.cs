using System;
using DG.Tweening;
using Entity;
using GamePlaySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    [RequireComponent(typeof(Image))]
    public class PlayerPartySlotUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image slotBackground;
        [SerializeField] private Image icon;
        private Color stateNormalColor;
        private readonly Color deadColor = new Color(0.45f, 0f, 0f);
        
        private int slotIndex = -1;
        private Action<int> onSlotClick;

        private void Awake()
        {
            stateNormalColor = slotBackground.color;
        }

        public void OnPointerClick(PointerEventData eventData) {
            onSlotClick?.Invoke(slotIndex);
        }
        
        public void Initialize(int index, Action<int> onClick)
        {
            slotIndex = index;
            onSlotClick = onClick;
        }

        public void SetCharacter(Character character)
        {
            icon.DOFade(1, 0.25f);
            slotBackground.color = stateNormalColor;
            icon.sprite = character.sprite;
            character.OnDeathEvent += OnCharacterDie;
        }
        
        private void OnCharacterDie(Character character)
        {
            icon.DOFade(0, 0.25f);
            slotBackground.color = deadColor;
        }
    }
}