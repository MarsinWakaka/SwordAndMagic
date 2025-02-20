using System;
using System.Collections.Generic;
using Entity;
using GamePlaySystem.BattleFXSystem.Helper;
using UnityEngine;
using Utility.Singleton;

namespace InputSystem
{
    /// <summary>
    /// 用于获取当前鼠标操作的对戏对象，以及对戏对象的选择
    /// </summary>
    [Obsolete("类似功能开发中")]
    public class CursorTargetManager : SingletonMono<CursorTargetManager>
    {
        private PlayerInputProvider targetPlayerInputProvider;
        
        [Header("样式设置器")]
        [SerializeField] private HoverStyleHelper hoverHelper;
        // [SerializeField] private SelectStyleHelper selectHelper;
        
        // 选择的次数
        private readonly Dictionary<BaseEntity, int> _selectDict = new();
        public Action<BaseEntity,int, bool> OnEntitySelected;
        public Action<BaseEntity> OnEntityCancelSelected;
        
        protected override void OnAwake()
        {
            targetPlayerInputProvider = GetComponent<PlayerInputProvider>();
            // 通过GameEntityChooser获取选择的实体
            // targetPlayerInputProvider.onEntityClicked += SetEntitySelected;
            // targetPlayerInputProvider.OnRightMouseClick += CancelEntitySelected;
        }
        #region Hover Entity
        
        private BaseEntity hoverBaseEntity;
        public bool TryGetHover(out BaseEntity baseEntity)
        {
            if (hoverBaseEntity == null)
            {
                baseEntity = null;
                return false;
            }
            baseEntity = hoverBaseEntity;
            return true;
        }
        
        public void SetEntityHover(BaseEntity baseEntity, Vector2 position)
        {
            hoverBaseEntity = baseEntity;
            hoverHelper.SetHoverFX(position);
        }
        public void CancelHover()
        {
            hoverBaseEntity = null;
            hoverHelper.CancelHoverFX();
        }
        
        public BaseEntity BaseEntityHover => hoverBaseEntity;
        
        #endregion
        
        #region Select Entity
        
        private bool _isAllowMultiSelect = false;
        private int _maxSelectCount = 1;
        public void SetMultiSelect(bool allow, int maxSelectCount)
        {
            _isAllowMultiSelect = allow;
            _maxSelectCount = maxSelectCount;
        }
        
        [Obsolete("Use TryGetHover instead")]
        public bool TryGetFirstSelectedEntity(out BaseEntity baseEntity)
        {
            baseEntity = null;
            if (_selectDict.Count == 0) return false;
            var enumerator = _selectDict.Keys.GetEnumerator();
            baseEntity = enumerator.Current;
            enumerator.Dispose();
            return true;
        }
        
        public void GetSelectedEntities(out List<BaseEntity> entities)
        {
            entities = new List<BaseEntity>(_selectDict.Keys);
        }
        
        private void SetEntitySelected(BaseEntity baseEntity, bool shouldAppend)
        {
            // 1、左键点击物体
            // 2、是否按住叠加
            // 若是，且该物体已被选中，则追加选择次数，若未被选中，则选中该物体
            // 若不是，先取消其它物体的选中，再选中该物体
            // 是否追加选择
            
            if (shouldAppend)
            {
                if (!_selectDict.TryAdd(baseEntity, 1))
                    _selectDict[baseEntity]++;
                OnEntitySelected?.Invoke(baseEntity, _selectDict[baseEntity], true);
            }
            else
            {
                foreach (var selectedEntity in _selectDict.Keys)
                {
                    OnEntityCancelSelected?.Invoke(selectedEntity);
                }
                _selectDict.Clear();
                _selectDict.Add(baseEntity, 1);
                OnEntitySelected?.Invoke(baseEntity, 1, false);
            }
        }

        private void CancelEntitySelected(BaseEntity baseEntity)
        {
            // 1、右键点击物体
            // 若该物体已被选中，则减少该物体的选择次数，若选择次数为0，则取消选中
            // 若未被选中，则不做任何操作
            if (_selectDict.ContainsKey(baseEntity))
            {
                _selectDict[baseEntity]--;
                if (_selectDict[baseEntity] == 0)
                {
                    _selectDict.Remove(baseEntity);
                    OnEntityCancelSelected?.Invoke(baseEntity);
                }
                else
                {
                    OnEntitySelected?.Invoke(baseEntity, _selectDict[baseEntity], true);
                }
            }
        }

        public BaseEntity GetGameEntitySelected()
        {
            if(_selectDict.Keys.Count == 0) return null;
            var cur = _selectDict.Keys.GetEnumerator().Current;
            if (cur == null) Debug.LogError("未知错误");
            return cur;
        }
        
        #endregion
    }
}