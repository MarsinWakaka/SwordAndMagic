using System;
using ConsoleSystem;
using MyEventSystem;
using UnityEngine;

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
        private TileType _tileType;
        /// 是否被堵住了, 即是角色能否行走在上面
        private bool _isBlocked;
        /// 站在上面的实体，可以是角色，物体等
        public BaseEntity standOnObj;    
        
        public TileType TileType
        {
            get => _tileType;
            set  {
                if (_tileType == value) return;
                _tileType = value;
                OnTypeChanged?.Invoke(this, _tileType);
            }
        }
        public Action<Tile, TileType> OnTypeChanged;
        public bool IsBlocked => _isBlocked;
        public BaseEntity StandOnObj
        {
            get => standOnObj;
            set
            {
                if (standOnObj == value) return;
                if (value is Character character)
                {
                    _isBlocked = true;
                }
            }
        }
        
        public void OnCharacterEnter(Character character)
        {
            standOnObj = character;
            _isBlocked = true;
        }
        
        public void OnCharacterExit(Character character)
        {
            standOnObj = null;
            _isBlocked = false;
        }

        public void Initialize(TileType tileType, Vector3 position)
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