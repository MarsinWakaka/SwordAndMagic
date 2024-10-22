#define GameTest

using System.Collections.Generic;
using BattleSystem.Emporium;
using BattleSystem.Entity;
using BattleSystem.FactionSystem;
using Entity;
using Entity.Character;
using EventSystem;
using FactorySystem;
using InputSystem;
using UISystem;
using UISystem.Panel;
using UnityEngine;

namespace BattleSystem.DeploySystem
{
    public class DeployManager : MonoBehaviour
    {
        // 这里序列化只是暂时的，后面通过事件获取场景加载完毕后的角色数据
        [SerializeField]
        private List<Character> playerCharacterSellList = new();
        private int _characterSelectedIndex = -1;
        
        private bool TryGetSelectedCharacter(out Character character)
        {
            character = null;
            if (_characterSelectedIndex == -1) return false;
            character = playerCharacterSellList[_characterSelectedIndex];
            return true;
        }
        
        public void OnDeployStart()
        {
            // 面板结束按钮 --点击-> 玩家部署结束阶段
            UIManager.Instance.PushPanel(PanelType.CharacterEmporiumPanel);
            var emporiumPanel = UIManager.Instance.GetCurrentPanel() as CharacterEmporiumPanel;
            if (emporiumPanel == null)
            {
                print("获取角色商店面板失败，请检查");
                return;
            }
            // [Done]如何给部署面板可售卖的角色数据
            emporiumPanel.SetCharacterToBuyList(playerCharacterSellList);
            // [Done]监听UI事件，获取玩家选择的角色
            emporiumPanel.OnSelectChanged += (x) => {_characterSelectedIndex = x;};
            
#if GameTest
            // TODO 模拟测试
            emporiumPanel.OnSelectChanged(0);
            FactoryManager.Instance.CreateCharacter(
                FactionType.Player, 
                playerCharacterSellList[0].entityClassID, 
                new Vector2(0, 1)
                );
            FactoryManager.Instance.CreateCharacter(
                FactionType.Player, 
                playerCharacterSellList[1].entityClassID, 
                new Vector2(1, 3)
            );
            // 拿到角色商店面板，模拟点击结束按钮
            emporiumPanel.EndDeployButtonClicked();
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryDeploy();
            }
        }

        private bool TryDeploy()
        {
            // 1、获取玩家选择的角色
            if (!TryGetSelectedCharacter(out var selectedCharacter))
            {
                print("未选择角色");
                return false;
            }
            
            // 2、获取玩家点击的位置
            if (!CursorTargetManager.Instance.TryGetHover(out var entity))
            {
                print("未选择位置");
                return false;
            }
            var selectP = entity.transform.position;
            
            // 3、TODO 检查玩家是否可以部署在这个位置
            
            // 4、TODO 检查玩家队伍是否已满
            
            // 5、检查玩家金币是否充足
            if (selectedCharacter.sellPrice > PlayerData.Gold.Value)
            {
                print("金币不足");
                return false;
            }
            
            print($"部署成功:{selectedCharacter.characterName}");
            // 6、生成角色
            PlayerData.Gold.Value -= selectedCharacter.sellPrice;
            FactoryManager.Instance.CreateCharacter(FactionType.Player, selectedCharacter.entityClassID, selectP);
            return true;
        }

        public void OnDeployEnd()
        {
            // TODO 这里也可以做下延时，比如1秒后再开始战斗，给玩家一点时间看看自己的部署，同时UI可以做切换过渡
            
            EventCenter<GameStateEvent>.Instance.Invoke(GameStateEvent.GameStateBattleStart);
        }
    }
}