using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class StartPanel : BasePanel
    {
        [Header("按钮组")]
        public Button newGameButton;
        public Button loadGameButton;
        public Button settingButton;
        public Button exitButton;
        public Button editorButton;
        
        protected override void Awake()
        {
            base.Awake();
            newGameButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PushPanel(PanelType.StartNewJourneyPanel, null);
            });
            loadGameButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PushPanel(PanelType.SavePanel, null);
            });
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
        }
    }
}
