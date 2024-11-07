using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.SavePanelPart
{
    public class SaveSlot : MonoBehaviour
    {
        private Button slotButton;
        public Action<SaveSlot, UserSave> OnSaveSlotSelected;
        private UserSave _save;
        [SerializeField] private Text saveNameText;
        [SerializeField] private Text saveTimeText;

        private void Awake()
        {
            slotButton = GetComponent<Button>();
            slotButton.onClick.AddListener(() =>
            {
                OnSaveSlotSelected?.Invoke(this, _save);
            });
        }

        public void SetSaveSlot(UserSave save)
        {
            _save = save;
            saveNameText.text = save.saveName;
            saveTimeText.text = save.saveTime;
        }
    }
}