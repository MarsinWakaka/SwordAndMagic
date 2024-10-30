using System;
using ConsoleSystem;
using MyEventSystem;
using Unity.VisualScripting;
using UnityEngine;
using Utility;

namespace Entity
{
    public enum TileType
    {
        Unknown, // 未知
        Grass,  // 草地
        Water,  // 水
        Ice,    // 冰
        Bridge, // 桥
        Wood,   // 木质地面
        Burning,// 燃烧的木制地面
        Burned, // 烧焦的地面
        Lava,   // 熔岩
        Desert, // 沙漠
        Swamp,  // 沼泽
        Sand,
    }
    
    public class Tile : BaseEntity
    {
        [Header("瓦片状态显示")]
        [SerializeField] private TileType tileType;
        public TileType TileType
        {
            get => tileType;
            set  {
                if (tileType == value) return;
                tileType = value;
                OnTypeChanged?.Invoke(this, tileType);
            }
        }
        
        public Action<Tile, TileType> OnTypeChanged;
        
        // public readonly BindableProperty<TileType> TileType = new();
        public bool isOccupied;     // 是否被占据
        public Unit.Character occupier;
        
        public void Initialize(TileType tileType, Vector2 position)
        {
            TileType = tileType;
            transform.position = position;
            EventCenter<GameEvent>.Instance.Invoke<Tile>(GameEvent.OnTileCreated, this);
        }

        private void OnMouseDown()
        {
            MyConsole.Print("Tile Clicked: " + transform.position, MessageColor.Black);
            EventCenter<GameEvent>.Instance.Invoke<Vector2>(GameEvent.OnTileLeftClicked, transform.position);
        }
    }
}