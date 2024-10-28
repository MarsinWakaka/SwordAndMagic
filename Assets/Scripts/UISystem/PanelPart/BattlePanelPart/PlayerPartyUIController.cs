using System.Collections.Generic;
using Entity;
using Entity.Character;
using Entity.Unit;
using MyEventSystem;
using UnityEngine;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class PlayerPartyUIController : MonoBehaviour
    {
        [Header("左侧栏位--队伍角色设置")]
        [SerializeField] private RectTransform characterSlotContainer;
        [SerializeField] private PlayerPartySlotUI partySlotUIPrefab;
        private readonly List<PlayerPartySlotUI> viewSlots = new();
        [Header("底部栏位--选中角色设置: 角色状态 & 技能列表 & 结束回合按钮状态")]
        [SerializeField] private SelectedCharacterUI selectCharacterUI;
        
        private Character[] curCharacters;
        
        private void Awake()
        {
            EventCenter<GameEvent>.Instance.AddListener<Character[]>(GameEvent.UpdateUIOfPlayerParty, RefreshPlayerCharacterUI);
        }

        private void OnDestroy()
        {
            if (EventCenter<GameEvent>.IsInstanceNull) return;
            EventCenter<GameEvent>.Instance.RemoveListener<Character[]>(GameEvent.UpdateUIOfPlayerParty, RefreshPlayerCharacterUI);
        }

        /// <summary>
        /// 游戏开始时初始化玩家角色UI
        /// </summary>
        private void RefreshPlayerCharacterUI(Character[] newCharacters)
        {
            MakeSureSlots(newCharacters.Length);
            int characterCount = newCharacters.Length;
            for (int i = 0; i < characterCount; i++)
            {
                viewSlots[i].gameObject.SetActive(true);
                viewSlots[i].SetCharacter(newCharacters[i]);
            }
            curCharacters = newCharacters;
        }
        
        /// <summary>
        /// 来自UI点击时的更新，一般当用户点击角色时触发
        /// </summary>
        private void CharacterClickedHandle(int slotIndex)
        {
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnCharacterSlotUIClicked, curCharacters[slotIndex]);
        }
        
        private void MakeSureSlots(int length)
        {
            if (viewSlots.Count < length)
            {
                for (int i = viewSlots.Count; i < length; i++)
                {
                    var slot = Instantiate(partySlotUIPrefab, characterSlotContainer);
                    slot.Initialize(i, CharacterClickedHandle);
                    viewSlots.Add(slot);
                }
            }
            else if (viewSlots.Count > length)
            {
                for (int i = viewSlots.Count - 1; i >= length; i--)
                {
                    viewSlots[i].gameObject.SetActive(false);
                }
            }
        }
    }
}