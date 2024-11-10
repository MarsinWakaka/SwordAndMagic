using System.Collections.Generic;
using Configuration;
using Data;
using GamePlaySystem;
using GamePlaySystem.CharacterClassSystem;
using SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class StartNewJourneyPanel : BasePanel
    {
        [Header("View")]
        [SerializeField] private Image classChosenIcon;
        [SerializeField] private Text classChosenText;
        [Header("Control")]
        [SerializeField] private Button exitButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private InputField teamNameInputField;
        [SerializeField] private Button nextClassButton;
        [SerializeField] private Button previousClassButton;
        private List<CharacterClass> classes;
        private int classIndex;

        // // 职业初始数据
        // private List<CharacterProperty> classBaseData;
        
        protected override void Awake()
        {
            base.Awake();
            classes = ServiceLocator.Get<CharacterClassManager>().GetClasses();
            nextClassButton.onClick.AddListener(() => {
                classIndex = (classIndex + 1) % classes.Count;
                SetClassInfo(classIndex);
            });
            previousClassButton.onClick.AddListener(() => {
                classIndex = (classIndex - 1 + classes.Count) % classes.Count;
                SetClassInfo(classIndex);
            });
            exitButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PopPanel(PanelType.StartNewJourneyPanel);
            });
            confirmButton.onClick.AddListener(() =>
            {
                // 设置玩家名字
                var teamName = teamNameInputField.text;
                if (string.IsNullOrEmpty(teamName))
                {
                    Debug.LogError("队伍名不能为空");
                    return;
                }
                var config = ServiceLocator.Get<IConfigService>().ConfigData;
                PlayerManager.Instance.playerData = new PlayerData
                {
                    teamName = teamName,
                    characterProperties = new List<CharacterProperty>(),
                    levelUnlock = new int[config.levelCount],
                    levelStars = new int[config.levelCount]
                };
                GameSceneManager.LoadScene(new TacticScene());
            });
        }

        public override void OnEnter()
        {
            base.OnEnter();
            classIndex = 0;
            SetClassInfo(classIndex);
        }

        private void SetClassInfo(int index)
        {
            classChosenIcon.sprite = classes[index].classIcon;
            classChosenText.text = classes[index].className;
        }
    }
}