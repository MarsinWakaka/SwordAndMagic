using System.Collections.Generic;
using DG.Tweening;
using GamePlaySystem;
using GamePlaySystem.SkillSystem;
using MyEventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterSkillUI : MonoBehaviour
    {
        [Header("ICON FromAtlas")]
        [SerializeField] private Image roundOverImage;
        [SerializeField] private Image skillUI;
        private CanvasGroup canvasGroup;


        [Header("技能格子UI预制体 & 技能格子UI列表")]
        [SerializeField] private CharacterSkillSlotUI skillSlotUIPrefab;
        [SerializeField] private RectTransform skillSlotUIContainer;
        [SerializeField] private List<CharacterSkillSlotUI> skillSlotUIs = new();
        [SerializeField] private int skillSelectedIndex;
        [Header("技能选中UI")]
        [SerializeField] private SkillChosenUI skillChosenUI;
        
        private List<SkillSlot> skillSlots;
        private Character curCharacter;

        private float skillChosenUiy;
        private float skillListUIY;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            skillChosenUiy = skillChosenUI.transform.position.y;
            skillListUIY = transform.position.y;
        }

        internal void SetSpriteResources(Sprite iconAP, Sprite iconSP, Sprite iconCoolDownTime, Sprite iconRange, Sprite roundOverButtonSprite, Sprite skillChosenSprite, Sprite skillUISprite)
        {
            // skillUI.sprite = skillUISprite;
            roundOverImage.sprite = roundOverButtonSprite;
            skillChosenUI.SetSpriteResources(iconAP, iconSP, iconCoolDownTime, iconRange, skillChosenSprite);
        }
        
        public void Initialize()
        {
            EventCenter<GameEvent>.Instance.AddListener<BaseSkill>(GameEvent.OnSKillChosenStateEnter, OpenSkillChosenUI);
        }
        
        public void Uninitialize()
        {
            EventCenter<GameEvent>.Instance.RemoveListener<BaseSkill>(GameEvent.OnSKillChosenStateEnter, OpenSkillChosenUI);
        }
        public void InitializeSkillUI(Character character)
        {
            if (curCharacter != null) {
                // curCharacter.ReadyToEndEvent -= DisableSkillPanel;
                curCharacter.CancelReadyToEndEvent -= EnableSkillPanel;
                curCharacter.Property.AP.OnValueChanged -= UpdateSkillListUIsByAP;
                curCharacter.Property.SP.OnValueChanged -= UpdateSkillListUIsBySP;
            }
            curCharacter = character;
            // curCharacter.ReadyToEndEvent += DisableSkillPanel;
            curCharacter.CancelReadyToEndEvent += EnableSkillPanel;
            curCharacter.Property.AP.OnValueChanged += UpdateSkillListUIsByAP;
            curCharacter.Property.SP.OnValueChanged += UpdateSkillListUIsBySP;

            // 根据 角色状态 设置 技能栏状态
            if (curCharacter.IsOnTurn && !curCharacter.IsReadyToEndTurn)
                EnableSkillPanel();
            else
                DisableSkillPanel();
            
            skillSlots = curCharacter.Skills;;
            for (int i = 0; i < skillSlots.Count; i++)
            {
                var slot = GetSlot(i);
                slot.SetSkillSlot(skillSlots[i]);
                slot.CoolDownTimeUpdateHandle(skillSlots[i].RemainCoolDown.Value);
                slot.APUpdateHandle(curCharacter.Property.AP.Value);
                slot.SPUpdateHandle(curCharacter.Property.SP.Value);
            }
            RecycleUnusedSlot(skillSlots.Count);
            OpenSkillListUI();
        }
        
        private void UpdateSkillListUIsByAP(int newAP)
        {
            for (int i = 0; i < skillSlots.Count; i++) {
                GetSlot(i).APUpdateHandle(newAP);
            }
        }
        
        private void UpdateSkillListUIsBySP(int newSP)
        {
            for (int i = 0; i < skillSlots.Count; i++) {
                GetSlot(i).SPUpdateHandle(newSP);
            }
        }
        
        private void OnSkillSlotClick(int slotIndex)
        {
            skillSelectedIndex = slotIndex;
            // 这两者的顺序不能颠倒，否则可能出现UI处于技能选择状态，但角色状态机因为不满足技能释放条件而退出技能选择状态
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnSkillSlotUIClicked, skillSlots[slotIndex]);
        }
        
        private void DisableSkillPanel(){
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.5f;
        }
        private void EnableSkillPanel(){
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1;
        }

        private void OpenSkillListUI()
        {
            transform.DOMoveY(skillListUIY, 0.5f);
            skillChosenUI.transform.DOMoveY(skillChosenUiy, 0.5f);
            // 移除技能选择状态的监听
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.OnSKillChosenStateExit, OpenSkillListUI);
        }

        private void OpenSkillChosenUI(BaseSkill skill)
        {
            skillChosenUI.SetSkillChosenUI(skill);
            // TODO 临时处理，后面需要做些方便的调整
            transform.DOMoveY(skillChosenUiy, 0.5f);
            skillChosenUI.transform.DOMoveY(skillListUIY, 0.5f);
            // 开启技能选择状态的监听
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.OnSKillChosenStateExit, OpenSkillListUI);
        }

        #region 格子池子
        
        /// <summary>
        /// 回收未使用的技能格子
        /// </summary>
        /// <param name="startIndex"></param>
        private void RecycleUnusedSlot(int startIndex) {
            for (int i = startIndex; i < skillSlotUIs.Count; i++) {
                skillSlotUIs[i].gameObject.SetActive(false);
            }
        }

        private CharacterSkillSlotUI GetSlot(int index)
        {
            if (index >= skillSlotUIs.Count) {
                var skillSlotUI = Instantiate(skillSlotUIPrefab, skillSlotUIContainer);
                skillSlotUI.Initialize(index, OnSkillSlotClick);
                skillSlotUIs.Add(skillSlotUI);
            } else if (index < skillSlotUIs.Count) {
                skillSlotUIs[index].gameObject.SetActive(true);
            }
            return skillSlotUIs[index];
        }

        #endregion
    }
}