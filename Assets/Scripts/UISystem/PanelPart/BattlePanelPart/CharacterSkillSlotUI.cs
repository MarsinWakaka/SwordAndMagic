using System;
using GamePlaySystem.SkillSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterSkillSlotUI : MonoBehaviour, IPointerClickHandler
    {
        private Action<int> onSlotClick;
        private int slotIndex;
        private SkillSlot curSkillSlot;
        
        public void Initialize(int slotIndexGiven, Action<int> onSlotClickHandle)
        {
            slotIndex = slotIndexGiven;
            onSlotClick += onSlotClickHandle;
        }

        [Header("技能格子UI")]
        [SerializeField] private Image skillSlotUI;
        [SerializeField] private Image skillIcon;
        [SerializeField] private Image coolDownImage;
        [SerializeField] private Image slotMask;

        private bool isCoolDown;
        private bool isAPEnough;
        private bool isSPEnough;
        private readonly BindableProperty<bool> canRelease = new();

        private void Awake() => canRelease.OnValueChanged += (isReady) => slotMask.enabled = !isReady;

        // TODO 当角色不能释放技能时，技能图标变灰
        public void SetSkillSlot(SkillSlot newSkillSlot)
        {
            // [wrong]if (lastSkillSlot == skillSlot) return; 不难看出当角色只有一名时，技能冷却不能被刷新
            // 旧的先去，新的再来
            if (curSkillSlot != null) 
                curSkillSlot.RemainCoolDown.OnValueChanged -= CoolDownTimeUpdateHandle;
            curSkillSlot = newSkillSlot;
            skillIcon.sprite = curSkillSlot.skill.skillIcon;
            curSkillSlot.RemainCoolDown.OnValueChanged += CoolDownTimeUpdateHandle;
        }
        
        public void CoolDownTimeUpdateHandle(int newRemainCoolDownTime) {
            int maxCoolDown = curSkillSlot.skill.coolDown;
            coolDownImage.fillAmount = maxCoolDown > 0 ? (float)newRemainCoolDownTime / maxCoolDown : 0;
            isCoolDown = newRemainCoolDownTime == 0;
            canRelease.Value = isCoolDown && isAPEnough && isSPEnough;
        }
        
        public void APUpdateHandle(int newAP)
        {
            isAPEnough = newAP >= curSkillSlot.skill.AP_Cost;
            canRelease.Value = isCoolDown && isAPEnough && isSPEnough;
        }
        
        public void SPUpdateHandle(int newSP)
        {
            isSPEnough = newSP >= curSkillSlot.skill.SP_Cost;
            canRelease.Value = isCoolDown && isAPEnough && isSPEnough;
        }

        /// <summary>
        /// 对象回收前调用，用于解绑事件
        /// </summary>
        public void OnDisable()
        {
            // TODO 思考是否会出现lastSkillSlot为空的情况
            // if (lastSkillSlot == null) return;
            // curSkillSlot.OnSkillUsed -= HandleSkillReleased;
            curSkillSlot.RemainCoolDown.OnValueChanged -= CoolDownTimeUpdateHandle;
            curSkillSlot = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onSlotClick?.Invoke(slotIndex);
        }
    }
}