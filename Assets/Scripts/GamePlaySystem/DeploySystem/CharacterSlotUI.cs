using System;
using Entity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GamePlaySystem.DeploySystem
{
    public class CharacterSlotUI : MonoBehaviour, IPointerClickHandler
    {
        private int _slotIndex;
        
        [SerializeField] private Image icon;
        [SerializeField] private UnityEngine.UI.Text sellPriceText;
        
        public Action<int> onSlotClicked;

        public void Init(int slotIndex) {
            _slotIndex = slotIndex;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onSlotClicked?.Invoke(_slotIndex);
        }
        
        public void SetSlot(Character character)
        {
            icon.sprite = character.sprite;
            sellPriceText.text = character.SellPrice.ToString();
        }
    }
}