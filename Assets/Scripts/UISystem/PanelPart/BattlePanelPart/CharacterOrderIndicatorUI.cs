using System.Collections.Generic;
using Entity.Character;
using MyEventSystem;
using UnityEngine;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterOrderIndicatorUI : MonoBehaviour
    {
        [SerializeField] List<CharacterOrderSlotUI> actionOrderSlots = new();
        
        private void Awake()
        {
            EventCenter<GameEvent>.Instance.AddListener<Character[]>(GameEvent.UpdateUIOfActionUnitOrder, InitOrderIndicatorUI);
        }

        private void OnDestroy()
        {
            if (EventCenter<GameEvent>.IsInstanceNull) return;
            EventCenter<GameEvent>.Instance.RemoveListener<Character[]>(GameEvent.UpdateUIOfActionUnitOrder, InitOrderIndicatorUI);
        }

        public void InitOrderIndicatorUI(Character[] units)
        {
            for(int i = 0; i < units.Length; i++)
            {
                var slot = actionOrderSlots[i];
                slot.SetSlot(units[i]);
            }
        }
    }
}