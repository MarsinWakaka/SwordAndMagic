using System.Collections.Generic;
using BattleSystem.EmporiumSystem;
using Entity;
using Entity.Character;
using Entity.Unit;
using MyEventSystem;
using UnityEngine;
using Utility;

namespace UISystem.PanelPart.CharacterEmporiumPart
{
    public class CharacterEmporiumUI : MonoBehaviour
    {
        [SerializeField] private CharacterSlotUI characterSlotUIPrefab;
        private readonly List<CharacterSlotUI> activeSlotUIs = new ();
        public readonly BindableProperty<int> SelectedSlotIndex = new ();

        private void Awake() {
            SelectedSlotIndex.Value = -1;
        }

        public void SetCharacterToBuyList(List<Character> characterList)
        {
            int characterListCount = characterList.Count;
            int activeCharacterSlotsCount = activeSlotUIs.Count;
            // 先回收多余的角色槽
            for (int i = characterListCount; i < activeCharacterSlotsCount; i++)
                RecycleSlot(activeSlotUIs[i]);
            // 利用已有的槽位
            for (int i = 0; i < activeCharacterSlotsCount; i++)
                activeSlotUIs[i].SetSlot(characterList[i]);
            // 槽位不足则取池子里获取
            for (int i = activeCharacterSlotsCount; i < characterListCount; i++)
            {
                CharacterSlotUI slotUI = GetSlot();
                slotUI.Init(activeSlotUIs.Count);
                slotUI.onSlotClicked += ChangeSlotSelect;
                slotUI.SetSlot(characterList[i]);
                activeSlotUIs.Add(slotUI);
            }
        }

        private readonly Stack<CharacterSlotUI> characterSlotPool = new();
        private CharacterSlotUI GetSlot()
        {
            CharacterSlotUI slotUI = null;
            if (characterSlotPool.Count == 0)
            {
                slotUI = Instantiate(characterSlotUIPrefab, transform);
            } else {
                slotUI = characterSlotPool.Pop();
                slotUI.gameObject.SetActive(true);
            }
            return slotUI;
        }
        
        private void RecycleSlot(CharacterSlotUI slotUI)
        {
            slotUI.gameObject.SetActive(false);
            characterSlotPool.Push(slotUI);
        }
        private void ChangeSlotSelect(int index)
        {
            if (SelectedSlotIndex.Value == index)
                SelectedSlotIndex.Value = -1;
            else 
                SelectedSlotIndex.Value = index;
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.DeployCharacterSelected, SelectedSlotIndex.Value);
        }
        
        public Vector3 GetActiveSlotUIPosition(int index)
        {
            return activeSlotUIs[index].transform.position;
        }
    }
}