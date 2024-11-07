using UISystem.PanelPart.MainPanelPart;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class MainMenusPanel : BasePanel
    {
        [Header("按钮组")]
        public Button loadGameButton;
        public Button newGameButton;
        public Button settingButton;
        public Button exitButton;
        public Button editorButton;
        [Header("关卡选择器")]
        [SerializeField] private LevelChooseUI levelChooseUI;
        
        protected override void Awake()
        {
            base.Awake();
            loadGameButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PushPanel(PanelType.SavePanel, null);
            });
            newGameButton.onClick.AddListener(OpenLevelChooseUI);
            settingButton.onClick.AddListener(() =>
            {
                print("TODO LoadSaveByName 设置面板");
            });
            exitButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
            editorButton.onClick.AddListener(() =>
            {
                SceneSystem.GameSceneManager.LoadScene(new SceneSystem.EditorScene());
            });
            
            levelChooseUI.OnCancelButtonClick += CloseLevelChooseUI;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            CloseLevelChooseUI();
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
