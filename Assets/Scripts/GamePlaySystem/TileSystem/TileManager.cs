using System.Collections.Generic;
using Entity;
using MyEventSystem;
using ResourcesSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlaySystem.TileSystem
{
    public interface ITileManager
    {
        public Tile[,] GetTiles();
        public Dictionary<TileType, TileData> GetTileDict();
        public bool HasTile(int x, int y);
        public bool InBorder(int x, int y);
    }
    
    public class TileManager : MonoBehaviour, ITileManager
    {
        private const int MapWidth = 30;
        private const int MapHeight = 30;
        private readonly Dictionary<TileType, TileData> _tileDict = new();
        private readonly Tile[,] _tiles = new Tile[MapWidth, MapHeight]; // 后续从关卡数据中读取
        
        [SerializeField] private List<TileData> tileData = new();
        private void Awake()
        {
            foreach (var data in tileData)
            {
                _tileDict.Add(data.tileType, data);
            }
            EventCenter<GameEvent>.Instance.AddListener<Tile>(GameEvent.OnTileCreated, RegisterTile);
        }
        
        // public TileManager()
        // {
        //     LoadTiles();
        //     EventCenter<GameEvent>.Instance.AddListener<Tile>(GameEvent.OnTileCreated, RegisterTile);
        // }
        
        public Tile[,] GetTiles() => _tiles;
        public Dictionary<TileType, TileData> GetTileDict() => _tileDict;
        public bool HasTile(int x, int y) => _tiles[x, y] != null;

        public bool InBorder(int x, int y) => x is >= 0 and < MapWidth && y is >= 0 and < MapHeight;
        
        // public void OnRoundStart() // TODO 未来增添地形扩散。比如燃烧的地面会燃烧周围的地面 

        private void RegisterTile(Tile tile)
        {
            // TODO 注册瓦片
            var pos = tile.transform.position;
            _tiles[(int) pos.x, (int) pos.y] = tile;
            // TODO 根据其类型，更新瓦片数据
            tile.OnTypeChanged += UpdateTileType;
            UpdateTileType(tile, tile.TileType); // 先触发一次
        }
        
        private void UnregisterTile(Tile tile)
        {
            // TODO 注销瓦片
        }

        private void UpdateTileType(Tile tile, TileType newType)
        {
            // TODO 更新瓦片类型
            tile.TileType = newType;
            tile.Renderer.sprite = _tileDict[newType].tileSprite;
            // TODO 新瓦片能否站人？如果不能且有人站在上面，需要对人物进行处理
            // TODO 燃烧瓦片对周围瓦片的影响
        }
        
        // TODO 读取瓦片数据，最好从EXCEL表读取,或者JSON表里读取
        private void LoadTiles()
        {
            ServiceLocator.Get<IResourceManager>().LoadAllResourcesAsyncByTag<TileData>("TileData", (datas) =>
            {
                foreach (var data in datas) {
                    _tileDict.Add(data.tileType, data);
                }
            });
        }
    }
}