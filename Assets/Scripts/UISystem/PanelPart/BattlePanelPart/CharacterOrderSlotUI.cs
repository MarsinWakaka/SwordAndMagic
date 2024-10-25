using BattleSystem.FactionSystem;
using Entity.Character;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterOrderSlotUI : MonoBehaviour
    {
        // 427B3A
        private static Color AllyColor = new Color(0.26f, 0.48f, 0.23f);
        private static Color EnemyColor = new Color(0.66f, 0.24f, 0.19f);
        
        [SerializeField] private Image bgColor;
        [SerializeField] private Image icon;

        public void SetSlot(Character character)
        {
            icon.sprite = character.sprite;
            bgColor.color = character.Faction.Value == FactionType.Player ? AllyColor : EnemyColor;
        }
    }
}