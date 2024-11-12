using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

        // TEST
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.R))
        //     {
        //         StartCoroutine(EaseInOrderIndicatorUI());
        //     }
        // }
        //
        // private IEnumerator EaseInOrderIndicatorUI()
        // {
        //     var screenWidth = Screen.width;
        //     var interval = new WaitForSeconds(0.03f);
        //     foreach (var slot in actionOrderSlots)
        //     {
        //         yield return interval;
        //         slot.transform.DOMoveX(100 + screenWidth, 0.25f)
        //             .From().SetEase(Ease.OutBack)
        //             .SetRelative();
        //     }
        //     yield return null;
        // }

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
            // StartCoroutine(EaseInOrderIndicatorUI());
        }
    }
}