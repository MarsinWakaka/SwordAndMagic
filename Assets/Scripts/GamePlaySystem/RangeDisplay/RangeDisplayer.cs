using System.Collections.Generic;
using GamePlaySystem.TileSystem.Navigation;
using UnityEngine;
using UnityEngine.Pool;

namespace GamePlaySystem.RangeDisplay
{
    public enum RangeType
    {
        None,
        Movement,
        AttackRange
    }
    
    public class RangeDisplayHelper : MonoBehaviour
    {
        [SerializeField] private Color color;
        #region 功能代码
        
        private readonly Stack<RangeSlot> _activeSlots = new();
        
        public void ShowRange(List<PathNode> pathNodes, RangeType type)
        {
            while (_activeSlots.Count > 0)
                _rangeSlotPool.Release(_activeSlots.Pop());
            if (type == RangeType.None) return;
            foreach (var node in pathNodes)
            {                        
                var slot = _rangeSlotPool.Get();
                slot.transform.position = new Vector3(node.PosX, node.PosY, 0);
                slot.SetColor(color);
            }
            Debug.Log($"_activeSlots: {_activeSlots.Count}");
        }
        
        public void ShowRange(List<Vector2Int> positions, RangeType type)
        {
            while (_activeSlots.Count > 0)
                _rangeSlotPool.Release(_activeSlots.Pop());
            if (type == RangeType.None) return;
            foreach (var pos in positions)
            {                        
                var slot = _rangeSlotPool.Get();
                slot.transform.position = new Vector3(pos.x, pos.y, 0);
                slot.SetColor(color);
            }
        }

        #endregion
        
        [SerializeField] private RangeSlot rangeSlotPrefab;
        // 池化格子
        private IObjectPool<RangeSlot> _rangeSlotPool;

        private void Awake()
        {
            _rangeSlotPool = new ObjectPool<RangeSlot>(
                CreateRangeSlot, 
                GetRangeSlot, 
                ReleaseRangeSlot
            );
        }

        private RangeSlot CreateRangeSlot()
        {
            return Instantiate(rangeSlotPrefab, transform);
        }

        private void GetRangeSlot(RangeSlot slot)
        {
            slot.gameObject.SetActive(true);
            _activeSlots.Push(slot);
        }

        private void ReleaseRangeSlot(RangeSlot slot)
        {
            slot.gameObject.SetActive(false);
        }
    }
}