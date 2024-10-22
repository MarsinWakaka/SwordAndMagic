using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeprecatedBattleSystem
{
     /// <summary>
     /// 与Excel的数据相关
    /// </summary>
    [Serializable]
    public struct CellData
    {
        public Sprite cellSprite;
        public bool isMoveable;
        public bool canObstacleAttack;
    }

    public class Cell : MonoBehaviour
    {
        private Transform _transform;
        private SpriteRenderer _spriteRenderer;

        public GameObject standedGo;

        [FormerlySerializedAs("MoveableGO")] public GameObject moveableGo;
        [FormerlySerializedAs("AttackableGO")] public GameObject attackableGo;

        public CellData cellData;
        public int cost;  // 移动到这个格子的消耗
        
        private BattleManager _battleManager;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_spriteRenderer == null)
            {
                _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                Debug.Log("Cell: SpriteRenderer added!");
            }
        }

        public void Init(CellData cell, BattleManager battleManager)
        {
            cellData = cell;
            _spriteRenderer.sprite = cell.cellSprite;
            _battleManager = battleManager;
        }

        private void OnMouseDown()
        {
            if (!cellData.isMoveable) return;
            if (_battleManager == null) return;

            if (moveableGo.activeSelf)
            {
                _battleManager.TryApplyMove(_transform.position);
            }
        }
    }
}
