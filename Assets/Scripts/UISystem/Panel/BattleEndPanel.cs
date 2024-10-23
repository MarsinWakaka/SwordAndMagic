using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class BattleEndPanel : BasePanel
    {
        // [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        
        protected override void Awake()
        {
            base.Awake();
            exitButton.onClick.AddListener(OnClickExitButton);
        }

        private void OnClickExitButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}