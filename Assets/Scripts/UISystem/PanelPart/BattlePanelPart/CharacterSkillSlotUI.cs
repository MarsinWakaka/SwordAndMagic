using System;
using Entity.Character;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterSkillSlotUI : MonoBehaviour, IPointerClickHandler
    {
        private Action<int> onSlotClick;
        private int slotIndex;
        private SkillSlot lastSkillSlot;
        
        [Header("技能格子UI")]
        [SerializeField] private Image skillSlotUI;
        [SerializeField] private Image skillIcon;
        [SerializeField] private Image coolDownImage;
        
        public void Initialize(int slotIndexGiven, Action<int> onSlotClickHandle)
        {
            slotIndex = slotIndexGiven;
            onSlotClick += onSlotClickHandle;
        }
        
        public void SetSkillSlot(SkillSlot skillSlot)
        {
            //// if (lastSkillSlot == skillSlot) return; 此代码会导致BUG，技能无法刷新
            if (lastSkillSlot != null) lastSkillSlot.OnSkillUsed -= HandleSkillReleased;
            
            // skillBackground.color // TODO 根据技能类型显示不同背景色
            skillIcon.sprite = skillSlot.skill.skillIcon;
            skillSlotUI.raycastTarget = skillSlot.remainCoolDown == 0;
            coolDownImage.fillAmount = skillSlot.remainCoolDown / (float)skillSlot.skill.coolDown;
            
            skillSlot.OnSkillUsed += HandleSkillReleased;
            lastSkillSlot = skillSlot;
        }
        public void OnBeforeRecycle()
        {
            lastSkillSlot.OnSkillUsed -= HandleSkillReleased;
            lastSkillSlot = null;
        }
        private void HandleSkillReleased(){
            coolDownImage.fillAmount = lastSkillSlot.remainCoolDown / (float)lastSkillSlot.skill.coolDown;
            skillSlotUI.raycastTarget = lastSkillSlot.remainCoolDown == 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onSlotClick?.Invoke(slotIndex);
        }
    }
}