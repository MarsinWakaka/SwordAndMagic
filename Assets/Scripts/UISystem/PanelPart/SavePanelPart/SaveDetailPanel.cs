using GamePlaySystem;
using SaveSystem;
using SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.SavePanelPart
{
    public class SaveDetailPanel : MonoBehaviour
    {
        // 依赖
        private IUserSaveService userSaveService;
        [Header("按钮组")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button renameButton;
        [SerializeField] private Button loadButton;
        [Header("存档详细信息")]
        [SerializeField] private Text saveNameText;
        [SerializeField] private Text saveTimeText;
        [SerializeField] private Text teamNameText;
        [SerializeField] private Text goldCountText;
        [Header("子面板")]
        [SerializeField] private RenameSavePanel renameSavePanel;

        private UserData _data;
        private SaveSlot _slot;
        
        private void Awake()
        {
            userSaveService = ServiceLocator.Get<IUserSaveService>();
            renameSavePanel.OnSaveNameChanged += UpdateSaveName;
            renameSavePanel.gameObject.SetActive(false);
            saveButton.onClick.AddListener(() =>
            {
                userSaveService.Save(_data);
                Debug.Log("Save");
            });
            renameButton.onClick.AddListener(() =>
            {
                renameSavePanel.gameObject.SetActive(true);
                renameSavePanel.SetDefaultName(_data.saveName);
            });
            loadButton.onClick.AddListener(() =>
            {
                userSaveService.Load(_data);
                GameSceneManager.LoadScene(new TacticScene());
            });
        }
        
        public void DisplaySaveDetail(SaveSlot slot, UserData data)
        {
            _data = data;
            _slot = slot;
            saveNameText.text = data.saveName;
            saveTimeText.text = data.saveTime;
            var playerData = data.playerData;
            teamNameText.text = $"teamName:\t{playerData.teamName}";
            goldCountText.text = $"Gold:\t{playerData.teamGold}";
        }

        private void UpdateSaveName(string newSaveName)
        {
            // TODO 更新列表中的存档名
            userSaveService.DeleteSave(_data); // 删除旧存档
            _data.saveName = newSaveName;
            _slot.SetSaveSlot(_data);
            saveNameText.text = newSaveName;
            userSaveService.Save(_data);
        }
    }
}