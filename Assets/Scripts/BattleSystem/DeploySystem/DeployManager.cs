using System.Collections.Generic;
using BattleSystem.Emporium;
using BattleSystem.FactionSystem;
using Entity;
using Entity.Character;
using FactorySystem;
using MyEventSystem;
using UISystem;
using UISystem.Panel;
using UnityEngine;

namespace BattleSystem.DeploySystem
{
    public class DeployManager : MonoBehaviour
    {
        [Header("自动部署角色（测试用）")]
        [SerializeField] bool autoDeploy = false;
        
        [Header("角色售卖列表")]
        // TODO 这里序列化只是暂时的，后面通过事件获取场景加载完毕后的角色数据
        [SerializeField] private List<Character> characterSellList = new();
        private int _characterSelectedIndex = -1;
        
        public void OnDeployStart()
        {
            // 面板结束按钮 --点击-> 玩家部署结束阶段
            UIManager.Instance.PushPanel(PanelType.CharacterEmporiumPanel, OnPushPanelComplete);
            EventCenter<GameEvent>.Instance.AddListener<BaseEntity>(GameEvent.OnEntityLeftClicked, TryDeploy);
        }

        private void OnPushPanelComplete()
        {
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.DeployCharacterResource, characterSellList);
            EventCenter<GameEvent>.Instance.AddListener<int>(GameEvent.DeployCharacterSelected, DeployCharacterSelected);
#if UNITY_EDITOR
            if (autoDeploy)
            {// TODO 模拟测试
                FactoryManager.Instance.CreateCharacter(
                    FactionType.Player, 
                    characterSellList[0].entityClassID, 
                    new Vector2(0, 1)
                );
                FactoryManager.Instance.CreateCharacter(
                    FactionType.Player, 
                    characterSellList[1].entityClassID, 
                    new Vector2(1, 3)
                );
                FactoryManager.Instance.CreateCharacter(
                    FactionType.Player, 
                    characterSellList[2].entityClassID, 
                    new Vector2(1, 4)
                );
            }
#endif
        }
        
        private void DeployCharacterSelected(int index)
        {
            _characterSelectedIndex = index;
        }

        private bool TryGetSelectedCharacter(out Character character)
        {
            character = null;
            if (_characterSelectedIndex == -1) 
                return false;
            
            character = characterSellList[_characterSelectedIndex];
            return true;
        }

        private void TryDeploy(BaseEntity entity)
        {
            // 获取玩家选择的角色
            if (!TryGetSelectedCharacter(out var selectedCharacter))
            {
                print("未选择角色");
                return;
            }
            
            var position = entity.transform.position;
            
            // TODO 检查玩家是否可以部署在这个位置
            // TODO 检查玩家队伍是否已满
            
            // 检查玩家金币是否充足
            if (selectedCharacter.sellPrice > PlayerData.Gold.Value) {
                print("金币不足");
                return;
            }
            
            print($"部署成功:{selectedCharacter.characterName}");
            // 6、生成角色
            PlayerData.Gold.Value -= selectedCharacter.sellPrice;
            FactoryManager.Instance.CreateCharacter(FactionType.Player, selectedCharacter.entityClassID, position);
        }

        public void OnDeployEnd()
        {
            if (UIManager.Instance.GetCurrentPanel() is CharacterEmporiumPanel) 
                UIManager.Instance.PopPanel();
            else
                Debug.LogError("当前面板不是CharacterEmporiumPanel，请检查面板加载顺序");
            
            // TODO 这里也可以做下延时，比如1秒后再开始战斗，给玩家一点时间看看自己的部署，同时UI可以做切换过渡
            EventCenter<GameEvent>.Instance.RemoveListener<BaseEntity>(GameEvent.OnEntityLeftClicked, TryDeploy);
            // 7、通知战斗开始
            EventCenter<GameStage>.Instance.Invoke(GameStage.BattleStart);
        }
    }
}