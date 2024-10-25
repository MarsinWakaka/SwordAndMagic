using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.CharacterEmporiumPart
{
    [RequireComponent(typeof(CharacterEmporiumUI))]
    public class EmporiumSlotSelectedStyle : MonoBehaviour
    {
        private CharacterEmporiumUI emporiumUI;
        [SerializeField] private RectTransform slotSelectStyle;
        private Image _selectSlotUI;
        private Color _colorWhileSelected;

        private void Awake()
        {
            _selectSlotUI = slotSelectStyle.GetComponent<Image>();
            _colorWhileSelected = _selectSlotUI.color;
            _selectSlotUI.color = Color.clear;
            emporiumUI = GetComponent<CharacterEmporiumUI>();
            emporiumUI.SelectedSlotIndex.OnValueChanged += SelectStyleHandle;
        }

        private void SelectStyleHandle(int index)
        {
            // 取消选择，关闭样式
            if (index == -1) {
                _selectSlotUI.DOColor(Color.clear, 0.2f);
            } else {
                _selectSlotUI.DOColor(_colorWhileSelected, 0.2f);
                slotSelectStyle.DOMove(emporiumUI.GetActiveSlotUIPosition(index), 0.2f);
            }
        }
    }
}