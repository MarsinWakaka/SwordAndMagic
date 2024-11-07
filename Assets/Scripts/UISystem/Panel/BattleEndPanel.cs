using SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class BattleEndPanel : BasePanel
    {
        [SerializeField] private Button exitButton;
        
        protected override void Awake()
        {
            base.Awake();
            exitButton.onClick.AddListener(OnClickExitButton);
        }

        private void OnClickExitButton()
        {
            GameSceneManager.LoadScene(new MainScene());
        }
    }
}