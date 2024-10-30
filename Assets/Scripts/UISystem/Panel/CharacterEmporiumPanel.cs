using System.Collections.Generic;
using Entity;
using Entity.Character;
using Entity.Unit;
using GamePlaySystem.EmporiumSystem;
using MyEventSystem;
using UISystem.PanelPart.CharacterEmporiumPart;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class CharacterEmporiumPanel : BasePanel
    {
        [Header("角色槽位")] 
        [SerializeField] private CharacterEmporiumUI emporiumPanel;
        [Header("金币选项")] 
        [SerializeField] private Text goldText;
        [SerializeField] private Button endButton; 
      
        #region Panel核心逻辑

        public override void OnEnter()
        {
            base.OnEnter();
            goldText.text = PlayerData.Gold.Value.ToString();
            PlayerData.Gold.OnValueChanged += UpdateCoins;
            // TODO 已知问题：玩家可以不部署直接进入游戏。
            endButton.interactable = false;
            endButton.onClick.AddListener(EndDeployButtonClicked);
            EventCenter<GameEvent>.Instance.AddListener(GameEvent.DeployCharacterSuccess, SetDeployButtonActive);
            EventCenter<GameEvent>.Instance.AddListener<List<Character>>(GameEvent.DeployCharacterResource, SetCharacterToBuyList);
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayerData.Gold.OnValueChanged -= UpdateCoins;
            endButton.onClick.RemoveListener(EndDeployButtonClicked);
            EventCenter<GameEvent>.Instance.RemoveListener(GameEvent.DeployCharacterSuccess, SetDeployButtonActive);
            EventCenter<GameEvent>.Instance.RemoveListener<List<Character>>(GameEvent.DeployCharacterResource, SetCharacterToBuyList);
        }

        #endregion
        
        private void SetDeployButtonActive() => endButton.interactable = true;
        
        private void UpdateCoins(int coins) => goldText.text = coins.ToString();

        private void SetCharacterToBuyList(List<Character> characterList) {
            emporiumPanel.SetCharacterToBuyList(characterList);
        }
        
        // 结束部署
        private void EndDeployButtonClicked()
        {
            EventCenter<GameStage>.Instance.Invoke(GameStage.PlayerDeployedEnd);
        } 
    }
}