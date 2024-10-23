using ConsoleSystem;
using Entity.Character;
using MyEventSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class SelectedCharacterUI : MonoBehaviour
    {
        [SerializeField] private CharacterStatusUI statusUI;
        [FormerlySerializedAs("skillSlotUIs")] 
        [SerializeField] private CharacterSkillUI skillsUI;
        [SerializeField] private Button endTurnButton;
        private Text endTurnButtonText;
        
        private Character selectedCharacter;
        
        private void Awake()
        {
            endTurnButtonText = endTurnButton.GetComponentInChildren<Text>();
            endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
            EventCenter<GameEvent>.Instance.AddListener<Character>(GameEvent.UpdateUIOfSelectedCharacter , UpdateUI);
        }

        private void UpdateUI(Character character)
        {
            // MyConsole.Print("[事件接受]" + GameEvent.UpdateUIOfSelectedCharacter, MessageColor.Black);
            // if (selectedCharacter == character) return;
            selectedCharacter = character;
            statusUI.SetStatusUI(selectedCharacter);
            skillsUI.SetSkillUI(selectedCharacter);
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