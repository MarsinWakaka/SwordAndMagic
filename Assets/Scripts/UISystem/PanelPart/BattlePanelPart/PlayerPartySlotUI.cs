using System;
using Entity.Character;
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
        
        private int slotIndex = -1;
        private Action<int> onSlotClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            onSlotClick?.Invoke(slotIndex);
        }
        
        public void Initialize(int index, Action<int> onClick)
        {
            slotIndex = index;
            onSlotClick = onClick;
        }

        public void SetCharacter(Character character)
        {
            // TODO 显示角色ICON
            icon.sprite = character.sprite;
            character.OnDeathEvent += OnCharacterDie;
        }
        
        private void OnCharacterDie(Character character)
        {
            // TODO 死亡时的UI处理
            var curColor = icon.color;
            curColor.a = 0.5f;
            icon.color = curColor;
            
            slotBackground.color = new Color(0.45f, 0f, 0f);
        }
    }
}