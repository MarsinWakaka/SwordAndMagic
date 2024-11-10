using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.SavePanelPart
{
    public class SaveSlot : MonoBehaviour
    {
        private Button slotButton;
        public Action<SaveSlot, UserData> OnSaveSlotSelected;
        public Action<SaveSlot, UserData> OnDeleteClicked;
        private UserData _data;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Text saveNameText;
        [SerializeField] private Text saveTimeText;

        private void Awake()
        {
            slotButton = GetComponent<Button>();
            slotButton.onClick.AddListener(() =>
            {
                OnSaveSlotSelected?.Invoke(this, _data);
            });
            deleteButton.onClick.AddListener(() =>
            {
                OnDeleteClicked?.Invoke(this, _data);
            });
        }

        public void SetSaveSlot(UserData data)
        {
            _data = data;
            saveNameText.text = data.saveName;
            saveTimeText.text = data.saveTime;
        }
    }
}