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
    
    public interface IRangeDisplayService
    {
        void ShowMoveRange(Dictionary<int, PathNode> pathNodes, int maxRange);
        void ShowMoveRange(List<PathNode> pathNodes, int maxRange);
        void ShowMoveRange(List<PathNode> pathNodes);
        void ShowAttackRange(List<Vector2Int> positions);
        void ShowAttackRange(HashSet<int> pathNodes);
        void CloseRangeDisplay();
    }
    
    public class RangeDisplayService : MonoBehaviour, IRangeDisplayService
    {
        [SerializeField] private Color movementRangeColor;
        [SerializeField] private Color attackRangeColor;
        #region 功能代码
        
        private readonly Stack<RangeSlot> _activeSlots = new();

        public void ShowMoveRange(Dictionary<int, PathNode> pathNodes, int maxRange)
        {
            // 关闭已有显示
            CloseRangeDisplay();
            // 决定颜色
            var color = movementRangeColor;
            var factor = 1f / (4 * maxRange);
            // 显示新的
            foreach (var node in pathNodes.Values)
            {                        
                var slot = _rangeSlotPool.Get();
                slot.transform.position = new Vector3(node.PosX, node.PosY, 0);
                color.a = node.Cost * factor;
                slot.SetColor(color);
            }
            Debug.Log($"_activeSlots: {_activeSlots.Count}");
        }

        public void ShowMoveRange(List<PathNode> pathNodes, int maxRange)
        {
            // 关闭已有显示
            CloseRangeDisplay();
            // 决定颜色
            var color = movementRangeColor;
            var factor = 1f / (4 * maxRange);
            // 显示新的
            foreach (var node in pathNodes)
            {                        
                var slot = _rangeSlotPool.Get();
                slot.transform.position = new Vector3(node.PosX, node.PosY, 0);
                color.a = node.Cost * factor;
                slot.SetColor(color);
            }
            Debug.Log($"_activeSlots: {_activeSlots.Count}");
        }
        
        public void ShowMoveRange(List<PathNode> pathNodes)
        {
            // 关闭已有显示
            CloseRangeDisplay();
            // 决定颜色
            var color = movementRangeColor;
            // 显示新的
            foreach (var node in pathNodes)
            {                        
                var slot = _rangeSlotPool.Get();
                slot.transform.position = new Vector3(node.PosX, node.PosY, 0);
                color.a = node.Cost / 20f;
                slot.SetColor(color);
            }
            Debug.Log($"_activeSlots: {_activeSlots.Count}");
        }
        
        public void ShowAttackRange(List<Vector2Int> positions)
        {
            // 关闭已有显示
            CloseRangeDisplay();
            // 决定颜色
            var color = attackRangeColor;
            // 显示新的
            foreach (var pos in positions)
            {                        
                var slot = _rangeSlotPool.Get();
                slot.transform.position = new Vector3(pos.x, pos.y, 0);
                slot.SetColor(color);
            }
            Debug.Log($"_activeSlots: {_activeSlots.Count}");
        }

        public void ShowAttackRange(HashSet<int> pathNodes)
        {
            // 关闭已有显示
            CloseRangeDisplay();
            // 决定颜色
            var color = attackRangeColor;
            // 显示新的
            foreach (var node in pathNodes)
            {
                NavigationService.GetXYFromIndexKey(node, out var x, out var y);
                var slot = _rangeSlotPool.Get();
                slot.transform.position = new Vector3(x, y, 0);
                slot.SetColor(color);
            }
            Debug.Log($"_activeSlots: {_activeSlots.Count}");
        }

        public void CloseRangeDisplay()
        {
            while (_activeSlots.Count > 0)
                _rangeSlotPool.Release(_activeSlots.Pop());
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