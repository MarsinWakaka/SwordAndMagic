using System;
using ConsoleSystem;
using EventSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Entity.Tiles
{
    public enum TileType
    {
        Void,   // 虚空
        Grass,  // 草地
        Water,  // 水
        Ice,    // 冰
        Wall,   // 石墙
        Wood,   // 木质地面
        Burning,// 燃烧的木制地面
        Burned, // 烧焦的地面
        Lava,   // 熔岩
        Desert, // 沙漠
        Swamp,  // 沼泽
        Sand,
        Unknown
    }
    
    public class Tile : BaseEntity
    {
        [Header("瓦片状态显示")]
        private TileType TileType;
        public bool isOccupied;     // 是否被占据
        public Character.Character occupier;

        private void Start()
        {
            EventCenter<GameEvent>.Instance.Invoke<Tile>(GameEvent.OnTileCreated, this);
        }

        private void OnMouseDown()
        {
            MyConsole.Print("Tile Clicked: " + transform.position, MessageColor.Black);
            EventCenter<GameEvent>.Instance.Invoke<Vector2>(GameEvent.OnTileLeftClicked, transform.position);
        }
    }
}