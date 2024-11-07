using System.Collections.Generic;
using SaveSystem;
using UISystem.PanelPart.SavePanelPart;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class SavePanel : BasePanel
    {
        // 依赖
        private IUserSaveService userSaveService;
        [Header("存档槽")]
        [SerializeField] private SaveSlot saveSlotPrefab;
        [SerializeField] private Transform saveSlotContainer;
        [Header("UI面板")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button createButton;
        [SerializeField] private SaveDetailPanel saveDetailPanel;
        private GameObject saveDetailPanelGo;
        private readonly Stack<SaveSlot> saveSlots = new();

        protected override void Awake()
        {
            base.Awake();
            userSaveService = ServiceLocator.Get<IUserSaveService>();
            saveDetailPanelGo = saveDetailPanel.gameObject;
            saveDetailPanelGo.SetActive(false);
            closeButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PopPanel(PanelType.SavePanel);
            });
            createButton.onClick.AddListener(() =>
            {
                // TODO 创建新存档
                var save = userSaveService.CreateNewSave("New Save");
                CreateSlot(save);
                Debug.Log("Create New Save");
            });
        }

        public override void OnEnter()
        {
            base.OnEnter();
            var saves = userSaveService.GetAllUserSaves();
            foreach (var save in saves)
            {
                CreateSlot(save);
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
            saveDetailPanelGo.SetActive(false);
            while (saveSlots.Count > 0) {
                var saveSlot = saveSlots.Pop();
                saveSlot.OnSaveSlotSelected -= OnSaveSlotSelected;
                Destroy(saveSlot.gameObject);
            }
        }

        private void OnSaveSlotSelected(SaveSlot slot, UserSave save)
        {
            if (!saveDetailPanelGo.activeSelf) saveDetailPanelGo.SetActive(true);
            saveDetailPanel.DisplaySaveDetail(slot, save);
            Debug.Log(save.saveName + " Selected");
        }
        
        private void CreateSlot(UserSave save)
        {
            var saveSlot = Instantiate(saveSlotPrefab, saveSlotContainer);
            saveSlot.SetSaveSlot(save);
            saveSlot.OnSaveSlotSelected += OnSaveSlotSelected;
            saveSlots.Push(saveSlot);
        }

    }
}