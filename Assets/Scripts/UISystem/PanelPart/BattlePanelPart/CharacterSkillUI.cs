using System.Collections.Generic;
using Entity.Character;
using MyEventSystem;
using UnityEngine;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterSkillUI : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

        [Header("技能格子UI预制体 & 技能格子UI列表")]
        [SerializeField] private CharacterSkillSlotUI skillSlotUIPrefab;
        [SerializeField] private RectTransform skillSlotUIContainer;
        [SerializeField] private List<CharacterSkillSlotUI> skillSlotUIs = new();
        [SerializeField] private int skillSelectedIndex;
        [Header("技能列表")]
        [SerializeField] private List<SkillSlot> skillSlots;
        
        private Character curCharacter;

        public void SetSkillUI(Character character)
        {
            if (curCharacter != null) {
                curCharacter.ReadyToEndEvent -= DisableSkillPanel;
                curCharacter.CancelReadyToEndEvent -= EnableSkillPanel;
            }
            character.ReadyToEndEvent += DisableSkillPanel;
            character.CancelReadyToEndEvent += EnableSkillPanel;
            curCharacter = character;

            // 根据角色状态设置技能栏是否可用
            if (character.IsOnTurn && !character.IsReadyToEndTurn)
                EnableSkillPanel();
            else
                DisableSkillPanel();
            
            skillSlots = character.Skills;;
            for (int i = 0; i < skillSlots.Count; i++) {
                GetSlot(i).SetSkillSlot(skillSlots[i]);
            }
            RecycleUnused(skillSlots.Count);
        }
        
        private void DisableSkillPanel(){
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.5f;
            // TODO 取消蒙上半透明黑色
        }
        private void EnableSkillPanel(){
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1;
            // TODO 蒙上半透明黑色
        }
        
        private void OnSkillSlotClick(int slotIndex)
        {
            skillSelectedIndex = slotIndex;
            // 向事件中心发送 当前选中技能被按下
            print($"当前选中技能: {skillSlots[slotIndex].skill.skillName}");
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnSkillSlotUIClicked, skillSlots[slotIndex]);
        }
        
        private void RecycleUnused(int startIndex) {
            for (int i = startIndex; i < skillSlotUIs.Count; i++) {
                skillSlotUIs[i].OnBeforeRecycle();
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
    }
}