using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UISystem
{
    public class MainMenusPanel : BasePanel
    {
        public Button loadGameButton;
        public Button newGameButton;
        public Button settingButton;
        public Button exitButton;
        
        protected override void Awake()
        {
            base.Awake();
            
            loadGameButton.onClick.AddListener(() =>
            {
                print("Load Game");
            });
            newGameButton.onClick.AddListener(NextLevel);
            settingButton.onClick.AddListener(() =>
            {
                
            });
            exitButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
            
#if UNITY_EDITOR
            Invoke(nameof(NextLevel), 0.1f);
#endif
        }
        
        private void NextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            UIManager.Instance.PopPanel();
        }
    }
}
