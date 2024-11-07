using System;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.SavePanelPart
{
    public class RenameSavePanel : MonoBehaviour
    {
        [SerializeField] private InputField saveNameInputField;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        public Action<string> OnSaveNameChanged;
        
        private void Awake()
        {
            confirmButton.onClick.AddListener(() =>
            {
                OnSaveNameChanged?.Invoke(saveNameInputField.text);
                gameObject.SetActive(false);
            });
            cancelButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }

        public void SetDefaultName(string oldName)
        {
            saveNameInputField.text = oldName;
        }
    }
}