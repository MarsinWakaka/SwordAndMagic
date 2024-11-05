using DG.Tweening;
using Entity;
using MyEventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class SelectedCharacterUI : MonoBehaviour
    {
        private RectTransform rectTransform;
        private float originalPositionY;
        private float hidePositionY;
        private bool isFirstAfterInitialized;
        [Header("UI组件")]
        [SerializeField] private CharacterStatusUI statusUI;
        [SerializeField] private CharacterSkillUI skillsUI;
        [SerializeField] private Button endTurnButton;
        private Text endTurnButtonText;
        
        private Character selectedCharacter;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalPositionY = rectTransform.anchoredPosition.y;
            hidePositionY = originalPositionY - 300;
            endTurnButtonText = endTurnButton.GetComponentInChildren<Text>();
            endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
        }

        public void Initialize()
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, hidePositionY);
            isFirstAfterInitialized = true;
            EventCenter<GameEvent>.Instance.AddListener<Character>(GameEvent.UpdateUIOfSelectedCharacter , SetSelectedCharacterUI);
            statusUI.Initialize();
            skillsUI.Initialize();
        }
        
        public void Uninitialize()
        {
            EventCenter<GameEvent>.Instance.RemoveListener<Character>(GameEvent.UpdateUIOfSelectedCharacter , SetSelectedCharacterUI);
            statusUI.Uninitialize();
            skillsUI.Uninitialize();
        }

        private void SetSelectedCharacterUI(Character character)
        {
            if (isFirstAfterInitialized) {
                rectTransform.DOAnchorPosY(originalPositionY, 0.5f);
                isFirstAfterInitialized = false;
            }
            selectedCharacter = character;
            statusUI.SetStatusUI(selectedCharacter);
            skillsUI.InitializeSkillUI(selectedCharacter);
            RefreshEndButtonState();
        }
        
        private const string EndTurnText = "结束";
        private const string CancelEndTurnText = "取消结束";

        private void OnEndTurnButtonClick()
        {
            selectedCharacter.SwitchEndTurnReadyState();
            RefreshEndButtonState();
        }

        private void RefreshEndButtonState()
        {
            var character = selectedCharacter;
            endTurnButtonText.text = character.IsReadyToEndTurn ? CancelEndTurnText : EndTurnText;
            // 如果角色不在行动状态，则不可点击
            endTurnButton.interactable = character.IsOnTurn;
        }
    }
}