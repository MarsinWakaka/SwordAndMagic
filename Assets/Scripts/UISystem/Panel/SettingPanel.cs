using SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class SettingPanel : BasePanel
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button savePanelButton;
        [SerializeField] private Button backToStartMenu;

        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PopPanel(PanelType.SettingPanel);
            });
            savePanelButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PushPanel(PanelType.SavePanel, null);
            });
            backToStartMenu.onClick.AddListener(() =>
            {
                GameSceneManager.LoadScene(new StartScene());
            });
        }
    }
}