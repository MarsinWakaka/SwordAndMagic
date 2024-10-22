using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleSystem.EmporiumSystem
{
    public struct SlotData
    {
        public int characterClass;
        public Sprite sprite;
        public int sellPrice;
    }
    
    public class CharacterSlot : MonoBehaviour, IPointerClickHandler
    {
        private int _slotIndex;
        
        [SerializeField] private Image icon;
        [SerializeField] private Text sellPriceText;
        
        public Action<int> onSlotClicked;

        public void Init(int slotIndex)
        {
            _slotIndex = slotIndex;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onSlotClicked?.Invoke(_slotIndex);
        }
        
        public void SetSlot(SlotData slotData)
        {
            icon.sprite = slotData.sprite;
            sellPriceText.text = slotData.sellPrice.ToString();
        }
    }
}