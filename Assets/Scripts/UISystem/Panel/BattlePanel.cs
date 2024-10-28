using SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class BattlePanel : BasePanel
    {
        [SerializeField] private Button exitButton;

        protected override void Awake()
        {
            base.Awake();
            exitButton.onClick.AddListener(() => { GameSceneManager.LoadScene(new MainScene()); });
        }
    }
}