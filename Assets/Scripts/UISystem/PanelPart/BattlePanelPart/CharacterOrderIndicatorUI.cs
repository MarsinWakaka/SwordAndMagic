using System.Collections.Generic;
using Entity;
using GamePlaySystem;
using MyEventSystem;
using UnityEngine;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterOrderIndicatorUI : MonoBehaviour
    {
        [SerializeField] List<CharacterOrderSlotUI> actionOrderSlots = new();
        
        public void Initialize()
        {
            EventCenter<GameEvent>.Instance.AddListener<Character[]>(GameEvent.UpdateUIOfActionUnitOrder, InitOrderIndicatorUI);
        }

        public void Uninitialize()
        {
            EventCenter<GameEvent>.Instance.RemoveListener<Character[]>(GameEvent.UpdateUIOfActionUnitOrder, InitOrderIndicatorUI);
        }

        private void InitOrderIndicatorUI(Character[] units)
        {
            for(int i = 0; i < units.Length; i++)
            {
                var slot = actionOrderSlots[i];
                slot.SetSlot(units[i]);
            }
        }
    }
}