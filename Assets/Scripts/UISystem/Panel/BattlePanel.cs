using UISystem.PanelPart.BattlePanelPart;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class BattlePanel : BasePanel
    {
        [SerializeField] private CharacterOrderIndicatorUI characterOrderUI;
        [SerializeField] private PlayerPartyUI playerPartyUI;
        [SerializeField] private SelectedCharacterUI selectedCharacterUI;
        [SerializeField] private InvestigationUI investigationUI;
        [SerializeField] private Button exitButton;

        protected override void Awake()
        {
            base.Awake();
            exitButton.onClick.AddListener(() => { UIManager.Instance.PushPanel(PanelType.SettingPanel, null); });
        }

        public override void OnEnter()
        {
            base.OnEnter();
            playerPartyUI.Initialize();
            characterOrderUI.Initialize();
            selectedCharacterUI.Initialize();
            investigationUI.Initialize();
        }

        public override void OnExit()
        {
            base.OnExit();
            playerPartyUI.Uninitialize();
            characterOrderUI.Uninitialize();
            selectedCharacterUI.Uninitialize();
            investigationUI.Uninitialize();
        }
    }
}