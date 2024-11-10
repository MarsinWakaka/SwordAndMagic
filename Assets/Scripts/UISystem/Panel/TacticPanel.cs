using GamePlaySystem;
using UISystem.PanelPart.TacticPanelPart;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class TacticPanel : BasePanel
    {
        [Header("关卡选择器")]
        [SerializeField] private Button settingButton;
        [SerializeField] private Button shopPanelButton;
        [SerializeField] private Button levelChosenButton;
        [SerializeField] private LevelChooseUI levelChooseUI;
        [Header("用户信息")] 
        [SerializeField] private Text teamName;
        [SerializeField] private Text goldCount;
        
        protected override void Awake()
        {
            base.Awake();
            settingButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PushPanel(PanelType.SettingPanel, null);
            });
            shopPanelButton.onClick.AddListener(() =>
            {
                // TODO 测试存档功能
                var user = PlayerManager.Instance.playerData;
                user.teamGold += 100;
                goldCount.text = user.teamGold.ToString();
            });
            levelChosenButton.onClick.AddListener(OpenLevelChooseUI);
            levelChooseUI.OnCancelButtonClick += CloseLevelChooseUI;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            CloseLevelChooseUI();
            var user = PlayerManager.Instance.playerData;
            if (user == null)
            {
                Debug.LogError("用户数据为空");
                return;
            }
            teamName.text = user.teamName;
            goldCount.text = user.teamGold.ToString();
        }
        
        private void CloseLevelChooseUI()
        {
            levelChooseUI.gameObject.SetActive(false);
        }
        
        private void OpenLevelChooseUI()
        {
            levelChooseUI.gameObject.SetActive(true);
            // SceneSystem.GameSceneManager.LoadScene(new SceneSystem.BattleScene(1));
        }
    }
}