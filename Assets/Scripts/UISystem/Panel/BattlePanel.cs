using System.Collections.Generic;
using BattleSystem.Entity.Character;
using Entity.Character;
using EventSystem;
using UISystem.PanelPart.BattlePanelPart;
using UnityEngine;
using UnityEngine.Serialization;

namespace UISystem.Panel
{
    public class BattlePanel : BasePanel
    {
        [SerializeField] private PlayerPartyUIController playerPartyUIController;
        [SerializeField] private CharacterOrderIndicatorUI orderIndicatorUI;
        // [SerializeField] private InvestigationUI investigationUI;
        
        public void UpdateCharacterOrderUI(Character[] units)
        {
            orderIndicatorUI.InitOrderIndicatorUI(units);
        }
    }
}