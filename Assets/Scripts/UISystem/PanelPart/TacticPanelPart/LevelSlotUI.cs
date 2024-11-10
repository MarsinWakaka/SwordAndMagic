using SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.TacticPanelPart
{
    public class LevelSlotUI : MonoBehaviour
    {
        [SerializeField] private Text indexTest;
        private Button button;

        private int levelIndex;

        public void Initialize(int index)
        {
            indexTest.text = index.ToString();
            levelIndex = index;
            button.interactable = index != -1;
        }

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                GameSceneManager.LoadScene(new BattleScene(levelIndex));
            });
        }
    }

}