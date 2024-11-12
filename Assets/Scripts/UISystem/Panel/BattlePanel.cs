using UISystem.PanelPart.BattlePanelPart;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
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
            // string spriteAtlasPath = "Assets/GameRes/Atlas/BattleUIAtlas.spriteatlas";
            // SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(spriteAtlasPath);
            // if (spriteAtlas == null)
            // {
            //     Debug.LogError("SpriteAtlas is null，please check the path: " + spriteAtlasPath);
            //     return;
            // }
            // Sprite iconAP = spriteAtlas.GetSprite("ICON_AP");
            // Sprite iconSP = spriteAtlas.GetSprite("ICON_SP");
            // Sprite iconCoolDownTime = spriteAtlas.GetSprite("ICON_CoolDownTime");
            // Sprite iconRange = spriteAtlas.GetSprite("ICON_RANGE");
            // Sprite roundOverButton = spriteAtlas.GetSprite("RoundOverButton");
            // Sprite SKillChosenUI = spriteAtlas.GetSprite("SKillChosenUI");
            // Sprite skillUI = spriteAtlas.GetSprite("SkillUI");
            // selectedCharacterUI.SetSpriteResources(iconAP, iconSP, iconCoolDownTime, iconRange, roundOverButton, SKillChosenUI, skillUI);
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