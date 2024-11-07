using Entity;
using GamePlaySystem;
using GamePlaySystem.FactionSystem;
using MyEventSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterOrderSlotUI : MonoBehaviour, IPointerClickHandler
    {
        // 427B3A
        private static Color AllyColor = new Color(0.26f, 0.48f, 0.23f);
        private static Color EnemyColor = new Color(0.66f, 0.24f, 0.19f);
        
        [SerializeField] private Image bgColor;
        [SerializeField] private Image icon;
        [SerializeField] private RectTransform hpIndicator;

        private Character _character;
        [SerializeField] private int hpIndicatorMaxWidth;
        [SerializeField] private int hpIndicatorMaxHeight;
        private int _maxHp;

        public void SetSlot(Character character)
        {
            if (_character != null) _character.Property.HP.OnValueChanged -= HpChangeHandle;
            _character = character;
            icon.sprite = character.sprite;
            bgColor.color = character.Faction.Value == FactionType.Player ? AllyColor : EnemyColor;
            _maxHp = character.Property.HP_MAX.Value;
            HpChangeHandle(character.Property.HP.Value);
            _character.Property.HP.OnValueChanged += HpChangeHandle;
        }
        
        private void HpChangeHandle(int currentHp)
        {
            hpIndicator.sizeDelta = new Vector2(hpIndicatorMaxWidth,
                (float)(_maxHp -  currentHp) / _maxHp * hpIndicatorMaxHeight);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_character == null) return;
            EventCenter<GameEvent>.Instance.Invoke<Character>(GameEvent.OnCharacterSlotUIClicked, _character);
        }
    }
}