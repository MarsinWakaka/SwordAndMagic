using SaveSystem;
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
        [Header("存档信息")]
        [SerializeField] private Text saveNameText;
        [SerializeField] private Text saveTimeText;
        [Header("子面板")]
        [SerializeField] private RenameSavePanel renameSavePanel;

        private UserSave _save;
        private SaveSlot _slot;
        
        private void Awake()
        {
            userSaveService = ServiceLocator.Get<IUserSaveService>();
            renameSavePanel.OnSaveNameChanged += UpdateSaveName;
            renameSavePanel.gameObject.SetActive(false);
            saveButton.onClick.AddListener(() =>
            {
                userSaveService.Save(_save);
                Debug.Log("Save");
            });
            renameButton.onClick.AddListener(() =>
            {
                renameSavePanel.gameObject.SetActive(true);
                renameSavePanel.SetDefaultName(_save.saveName);
            });
            loadButton.onClick.AddListener(() =>
            {
                userSaveService.Load(_save);
                Debug.Log("Load");
            });
        }
        
        public void DisplaySaveDetail(SaveSlot slot, UserSave save)
        {
            _save = save;
            _slot = slot;
            saveNameText.text = save.saveName;
            saveTimeText.text = save.saveTime;
        }

        private void UpdateSaveName(string newSaveName)
        {
            // TODO 更新列表中的存档名
            userSaveService.DeleteSave(_save.saveName); // 删除旧存档
            _save.saveName = newSaveName;
            _slot.SetSaveSlot(_save);
            saveNameText.text = newSaveName;
            userSaveService.Save(_save);
        }
    }
}