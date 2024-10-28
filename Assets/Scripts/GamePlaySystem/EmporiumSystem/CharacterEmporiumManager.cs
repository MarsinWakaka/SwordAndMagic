// using System.Collections.Generic;
// using GamePlaySystem.Emporium;
// using GamePlaySystem.Entity;
// using GamePlaySystem.Entity.Creature;
// using UISystem.Panel;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace GamePlaySystem.EmporiumSystem
// {
//     /// <summary>
//     /// 这是一个角色商店管理器，负责管理角色商店的逻辑
//     /// </summary>
//     [RequireComponent(typeof(CharacterEmporiumPanel))]
//     public class CharacterEmporiumManager : MonoBehaviour
//     {
//         [FormerlySerializedAs("_characterToSellList")]
//         [SerializeField]
//         private List<Character> characterToSellList = new();
//         private int _slotIndex = -1;
//         
//         private CharacterEmporiumPanel emporiumPanel;
//         private void Awake()
//         {
//             // LoadCharacters();
//             emporiumPanel = GetComponent<CharacterEmporiumPanel>();
//             emporiumPanel.SetCharacterToBuyList(characterToSellList);
//             emporiumPanel.OnSelectChanged += OnSelectChanged;
//             // 像游戏管理器注册自己
//         }
//
//         private void OnSelectChanged(int slotIndex)
//         {
//             _slotIndex = slotIndex;
//         }
//         
//         public bool GetCharacterSelected(out Character character)
//         {
//             if (_slotIndex == -1)
//             {
//                 character = null;
//                 return false;
//             }
//             
//             character = characterToSellList[_slotIndex];
//             return true;
//         }
//     }
// }