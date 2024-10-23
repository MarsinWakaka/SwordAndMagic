using System;
using System.Collections.Generic;
using BattleSystem.Emporium;
using BattleSystem.EmporiumSystem;
using DG.Tweening;
using Entity.Character;
using MyEventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class CharacterEmporiumPanel : BasePanel
    {
        private List<Character> _characterToSellList;
        public List<CharacterSlot> characterSlots = new();
        public int selectedSlotIndex = -1;

        [SerializeField] private RectTransform slotSelectStyle;
        private Image _selectSlotUI;
        private Color _selectSlotColor;
        
        [Header("功能")]
        [SerializeField] private Text goldText;
        [SerializeField] private Button endButton; 
        
        public Action<int> OnSelectChanged;

        #region Panel核心逻辑

        public override void OnEnter()
        {
            base.OnEnter();
            
            InitCharacterSlots();
            _selectSlotUI = slotSelectStyle.GetComponent<Image>();
            _selectSlotColor = _selectSlotUI.color;
            _selectSlotUI.color = Color.clear;
            
            endButton.onClick.AddListener(EndDeployButtonClicked);
            
            goldText.text = PlayerData.Gold.Value.ToString();
            PlayerData.Gold.OnValueChanged += UpdateCoins;
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayerData.Gold.OnValueChanged -= UpdateCoins;
        }

        #endregion
        
        private void UpdateCoins(int coins) => goldText.text = coins.ToString();

        private void RefreshUI()
        {
            // 更新UI
            var characterCount = _characterToSellList.Count;
            for (var i = 0; i < characterCount; i++)
            {
                characterSlots[i].SetSlot(new SlotData()
                {
                    characterClass = _characterToSellList[i].entityClassID,
                    sprite = _characterToSellList[i].sprite,
                    sellPrice = _characterToSellList[i].sellPrice
                });
            }

            var slotsCount = characterSlots.Count;
            for (var j = characterCount; j < slotsCount; j++)
            {
                characterSlots[j].gameObject.SetActive(false);
            }
        }

        private void InitCharacterSlots()
        {
            var characterCount = characterSlots.Count;
            for (var i = 0; i < characterCount; i++)
            {
                // 初始化角色槽
                characterSlots[i].Init(i);
                characterSlots[i].onSlotClicked += ChangeSlotSelect;
            }
        }
        
        private void ChangeSlotSelect(int index)
        {
            // 取消选择，关闭样式
            if (selectedSlotIndex == index)
            {
                selectedSlotIndex = -1;
                _selectSlotUI.DOColor(Color.clear, 0.2f);
                return;
            }
            
            if (selectedSlotIndex == -1)
            {
                // 样式未开启
                selectedSlotIndex = index;
                slotSelectStyle.position = characterSlots[index].transform.position;
                _selectSlotUI.DOColor(_selectSlotColor, 0.2f);
            }
            else
            {
                // 样式切换
                selectedSlotIndex = index;
                slotSelectStyle.DOMove(characterSlots[index].transform.position, 0.2f);
            }
            OnSelectChanged?.Invoke(selectedSlotIndex);
            // DeployManager.Instance.OnPlayerCharacterSelected = _characterToSellList[selectedSlotIndex];
        }
        
        public void SetCharacterToBuyList(List<Character> characterList)
        {
            _characterToSellList = characterList;
            RefreshUI();
        }
        
        // 结束部署
        public void EndDeployButtonClicked()
        {
            // 触发部署结束事件// 关闭商店面板
            UIManager.Instance.PopPanel();
            EventCenter<GameStateEvent>.Instance.Invoke(GameStateEvent.GameStatePlayerDeployedEnd);
        } 
    }
}