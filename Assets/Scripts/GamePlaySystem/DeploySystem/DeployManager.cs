using System.Collections.Generic;
using Entity;
using GamePlaySystem.EmporiumSystem;
using GamePlaySystem.FactionSystem;
using GamePlaySystem.TileSystem;
using MyEventSystem;
using UISystem;
using UnityEngine;

namespace GamePlaySystem.DeploySystem
{
    public class DeployManager : MonoBehaviour
    {
        [Header("自动部署角色（测试用）")]
        [SerializeField] bool autoDeploy = false;
        
        [Header("角色售卖列表")]
        // TODO 这里序列化只是暂时的，后面通过事件获取场景加载完毕后的角色数据
        private int _characterSelectedIndex = -1;
        [SerializeField] private List<Character> characterSellList = new();
        
        private TileManager _tileManager;
        public void Initialize(TileManager tileManager, List<Character> charactersSold)
        {
            _tileManager = tileManager;
            characterSellList = charactersSold;
        }
        
        public void OnDeployStart()
        {
            // PlayerData.Gold.Value = 50;
            // 面板结束按钮 --点击-> 玩家部署结束阶段
            EventCenter<GameEvent>.Instance.AddListener<BaseEntity>(GameEvent.OnEntityLeftClicked, TryDeploy);
            UIManager.Instance.PushPanel(PanelType.CharacterEmporiumPanel, OnPushPanelComplete);
        }

        private void OnPushPanelComplete()
        {
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.DeployCharacterResource, characterSellList);
            EventCenter<GameEvent>.Instance.AddListener<int>(GameEvent.DeployCharacterSelected, DeployCharacterSelected);
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
            
            // 检查玩家金币是否充足
            if (selectedCharacter.sellPrice > PlayerData.Gold.Value) {
                print("金币不足");
                return;
            }
            
            print($"部署成功:{selectedCharacter.characterName}");
            // 6、生成角色
            PlayerData.Gold.Value -= selectedCharacter.sellPrice;
            ServiceLocator.Get<IEntityFactory>().CreateCharacter(selectedCharacter.entityID, position, FactionType.Player);
            // EntityFactory.Instance.CreateCharacter(FactionType.Player, selectedCharacter.entityID, position);
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.DeployCharacterSuccess);   // 激活部署面板的结束按钮
        }

        public void OnDeployEnd()
        {
            UIManager.Instance.PopPanel(PanelType.CharacterEmporiumPanel);
            // TODO 这里也可以做下延时，比如1秒后再开始战斗，给玩家一点时间看看自己的部署，同时UI可以做切换过渡
            EventCenter<GameEvent>.Instance.RemoveListener<BaseEntity>(GameEvent.OnEntityLeftClicked, TryDeploy);
            // 7、通知战斗开始
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.BattleStart);
        }
    }
}