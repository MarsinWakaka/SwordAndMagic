using System;
using System.Collections.Generic;
using BattleSystem.Style;
using DG.Tweening;
using InputSystem;
using UnityEngine;

namespace BattleSystem.BattleFXSystem.Helper
{
    public class SelectStyleHelper : MonoBehaviour
    {
        public GameObject selectStylePrefab;
        
        [SerializeField]
        private List<Sprite> numList = new();
        
        [Header("Style Settings")] 
        public float selectDuration;

        private readonly Dictionary<Guid, SelectStyle> _styleDict = new();

        private void Awake()
        {
            CursorTargetManager.Instance.OnEntitySelected += SetSelectedStyle;
            CursorTargetManager.Instance.OnEntityCancelSelected += CancelSelectedStyle;
        }
        
        private void ModifyNumSprite(SelectStyle style, int selectCount)
        {
            style.SetNumSprite(numList[selectCount]);
        }
        
        private void AppendSelectedStyle(global::Entity.BaseEntity baseEntity, int selectCount)
        {
            var newStyle = Get();
            _styleDict[baseEntity.EntityID] = newStyle; // 这一步包含了集合的添加
            ModifyNumSprite(newStyle, selectCount);
            newStyle.transform.DOMove(baseEntity.transform.position, selectDuration);
        }

        private void SetSelectedStyle(global::Entity.BaseEntity baseEntity, int selectCount, bool isModify)
        {
            bool hasContainsThis = _styleDict.ContainsKey(baseEntity.EntityID);
            if (hasContainsThis)
            {
                if (!isModify)
                {
                    ClearSelectedStyle();
                    AppendSelectedStyle(baseEntity, selectCount);
                }
                else
                {
                    ModifyNumSprite(_styleDict[baseEntity.EntityID], selectCount);
                }
            }
            else
            {
                AppendSelectedStyle(baseEntity, selectCount);
            }

        }

        private void CancelSelectedStyle(global::Entity.BaseEntity baseEntity)
        {
            var id = baseEntity.EntityID;
            var cancelTrans = _styleDict[id];
            // 回收
            Release(cancelTrans);
            _styleDict.Remove(id);
        }

        private void ClearSelectedStyle()
        {
            foreach (var style in _styleDict)
            {
                Release(style.Value);
            }

            _styleDict.Clear();
        }
        
        #region 对象池
        [SerializeField] private Transform styleRoot;
        private readonly Stack<SelectStyle> _styleStack = new();

        private void CreateEntitySelectedStyle()
        {
            _styleStack.Push(Instantiate(selectStylePrefab, styleRoot).GetComponent<SelectStyle>());
        }

        private SelectStyle Get()
        {
            if (_styleStack.Count == 0)
                CreateEntitySelectedStyle();
            var style = _styleStack.Pop();
            style.gameObject.SetActive(true);
            return style;
        }

        private void Release(SelectStyle style)
        {
            style.gameObject.SetActive(false);
            _styleStack.Push(style);
        }

        #endregion
    }
}