using System.Collections.Generic;
using SaveSystem;
using SceneSystem;
using UISystem.PanelPart.SavePanelPart;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace UISystem.Panel
{
    public class SavePanel : BasePanel
    {
        // 依赖
        private IUserSaveService userSaveService;
        [Header("存档槽")]
        [SerializeField] private SaveSlot saveSlotPrefab;
        private RectTransform saveSlotTransform;
        [SerializeField] private RectTransform saveSlotContainer;
        [Header("UI面板")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button createButton;
        [SerializeField] private SaveDetailPanel saveDetailPanel;
        private GameObject saveDetailPanelGo;
        private readonly List<SaveSlot> activeSlotList = new();
        private ObjectPool<SaveSlot> saveSlotPool;

        protected override void Awake()
        {
            base.Awake();
            saveSlotTransform = saveSlotPrefab.GetComponent<RectTransform>();
            userSaveService = ServiceLocator.Get<IUserSaveService>();
            saveDetailPanelGo = saveDetailPanel.gameObject;
            saveDetailPanelGo.SetActive(false);
            BuildObjectPool();
            closeButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PopPanel(PanelType.SavePanel);
            });
            createButton.onClick.AddListener(() =>
            {
                // TODO 已知问题，存档排列顺序应当反过来
                var save = userSaveService.CreateNewSave("New Save");
                saveSlotPool.Get().SetSaveSlot(save);
            });
        }

        private void BuildObjectPool()
        {
            saveSlotPool = new ObjectPool<SaveSlot>(
                () => {
                    var slot = Instantiate(saveSlotPrefab, saveSlotContainer);
                    return slot;
                }, 
                (slot) => {
                    slot.OnSaveSlotSelected += OnSaveSlotSelected;
                    slot.OnDeleteClicked += OnDeleteHandle;
                    slot.gameObject.SetActive(true);
                    activeSlotList.Add(slot);
                    saveSlotContainer.sizeDelta = new Vector2(
                        0, (saveSlotTransform.rect.height + 5) * activeSlotList.Count);
                }, 
                (slot) =>
                {
                    slot.OnSaveSlotSelected -= OnSaveSlotSelected;
                    slot.OnDeleteClicked -= OnDeleteHandle;
                    slot.gameObject.SetActive(false);
                    activeSlotList.Remove(slot);
                    saveSlotContainer.sizeDelta = new Vector2(
                        0, (saveSlotTransform.rect.height + 5) * activeSlotList.Count);
                }, 
                (slot) =>
                {
                    Destroy(slot.gameObject);
                }, 
                true, 
                5, 
                200
                );
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ClearAllSlot();
            var saves = userSaveService.GetAllUserSaves();
            foreach (var save in saves) {
                saveSlotPool.Get().SetSaveSlot(save);
            }
            createButton.interactable = GameSceneManager.GetCurrentScene() is not StartScene;
        }
        
        public override void OnExit()
        {
            base.OnExit();
            saveDetailPanelGo.SetActive(false);
            ClearAllSlot();
        }

        private void OnSaveSlotSelected(SaveSlot slot, UserData data)
        {
            if (!saveDetailPanelGo.activeSelf) saveDetailPanelGo.SetActive(true);
            saveDetailPanel.DisplaySaveDetail(slot, data);
        }

        private void ClearAllSlot() {
            while (activeSlotList.Count > 0) saveSlotPool.Release(activeSlotList[0]);
        }

        private void OnDeleteHandle(SaveSlot slot, UserData data)
        {
            userSaveService.DeleteSave(data);
            saveSlotPool.Release(slot);
        }
    }
}