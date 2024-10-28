using System;
using System.Collections.Generic;
using MyEventSystem;
using UnityEngine;

namespace Entity.Tiles
{
    public struct TileData
    {
        public string TypeName;     // 瓦片名称
        public string TileSprite;   // 瓦片贴图
        public string Description;  // 瓦片描述
        public bool CanWalkThrough; // 是否可行走通过
        public bool CanJumpThrough; // 是否可跳跃通过
        public bool CanFlyThrough;  // 是否可飞行通过
        public bool IsObstacle;     // 是否是同时阻挡攻击
        public bool IsAttackable;   // 是否可被攻击
        public int Cost;            // 移动消耗
        public int Durability;      // 耐久度 
    }
    
    public class TileManager : MonoBehaviour
    {
        private readonly Dictionary<TileType, TileData> _tileDict = new();
        private readonly Dictionary<Vector2Int, Tile> _tiles = new();
        
        private void Awake()
        {
            EventCenter<GameEvent>.Instance.AddListener<Tile>(GameEvent.OnTileCreated, RegisterTile);
            LoadTiles();
        }

        private void RegisterTile(Tile tile)
        {
            // TODO 注册瓦片
            var tilePosition = new Vector2Int((int) tile.transform.position.x, (int) tile.transform.position.y);
            _tiles[tilePosition] = tile;
        }
        
        private void UnregisterTile(Tile tile)
        {
            // TODO 注销瓦片
        }

        private void UpdateTileType(Tile tile, TileType newType)
        {
            // TODO 更新瓦片类型
            // TODO 新瓦片能否站人？如果不能且有人站在上面，需要对人物进行处理
            // TODO 燃烧瓦片对周围瓦片的影响
        }

        // TODO 提供寻路支持
        
        // TODO 读取瓦片数据，最好从EXCEL表读取
        private void LoadTiles()
        {
            // 读取地图数据
            _tileDict.Add(TileType.Void, new TileData
            {
                TypeName = "Void",
                CanWalkThrough = false,
                CanJumpThrough = true,
                CanFlyThrough = true,
                IsObstacle = false,
                IsAttackable = false,
                Cost = 1,
                Durability = 1
            });
            _tileDict.Add(TileType.Grass, new TileData
            {
                TypeName = "Grass",
                CanWalkThrough = true,
                CanJumpThrough = true,
                CanFlyThrough = true,
                IsObstacle = false,
                IsAttackable = false,
                Cost = 1,
                Durability = 1
            });
            _tileDict.Add(TileType.Water, new TileData
            {
                TypeName = "Water",
                CanWalkThrough = false,
                CanJumpThrough = true,
                CanFlyThrough = true,
                IsObstacle = false,
                IsAttackable = false,
                Cost = 2,
                Durability = 1
            });
            _tileDict.Add(TileType.Ice, new TileData
            {
                TypeName = "Ice",
                CanWalkThrough = true,
                CanJumpThrough = true,
                CanFlyThrough = true,
                IsObstacle = false,
                IsAttackable = false,
                Cost = 1,
                Durability = 1
            });
            _tileDict.Add(TileType.Wall, new TileData
            {
                TypeName = "Wall",
                CanWalkThrough = false,
                CanJumpThrough = false,
                CanFlyThrough = false,
                IsObstacle = true,
                IsAttackable = true,
                Cost = 1,
                Durability = 1
            });
        }
    }
}