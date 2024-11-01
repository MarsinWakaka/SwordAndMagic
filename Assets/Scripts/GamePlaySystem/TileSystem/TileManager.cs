using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace GamePlaySystem.TileSystem
{
    public class TileManager : MonoBehaviour
    {
        private const int MapWidth = 30;
        private const int MapHeight = 30;
        private readonly Dictionary<TileType, TileData> _tileDict = new();
        private readonly Tile[,] _tiles = new Tile[MapWidth, MapHeight]; // 后续从关卡数据中读取
        // TODO 从关卡数据中读取，从而去掉Mono
        [SerializeField] private List<TileData> tileData = new();
        [SerializeField] private Transform tileParent;
        private void Awake()
        {
            foreach (var data in tileData)
            {
                _tileDict.Add(data.tileType, data);
            }
        }
        
        public Tile[,] GetTiles() => _tiles;
        public Dictionary<TileType, TileData> GetTileDict() => _tileDict;
        public bool HasTile(int x, int y) => _tiles[x, y] != null;
        public bool InBorder(int x, int y) => x is >= 0 and < MapWidth && y is >= 0 and < MapHeight;
        
        // public void OnRoundStart() // TODO 未来增添地形扩散。比如燃烧的地面会燃烧周围的地面 

        public void RegisterTile(Tile tile)
        {
            // TODO 注册瓦片
            tile.transform.SetParent(tileParent);
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
        
        public bool InitCharacterPosOnTile(Character character)
        {
            var position = character.transform.position;
            var x = (int) position.x; var y = (int) position.y;
            if (!InBorder(x, y) || !HasTile(x, y)) return false;
            var tile = _tiles[x, y];
            if (tile.IsBlocked) {
                Debug.LogWarning("Character can't be placed on a blocked tile");
                return false;
            }
            tile.OnCharacterEnter(character);
            character.transform.position = position;
            return true;
        }
        
        // 处理角色移动后的瓦片更新
        public bool CharacterMove(Character character, Vector3 from, Vector3 to)
        {
            var fromX = (int) from.x; var fromY = (int) from.y;
            var toX = (int) to.x; var toY = (int) to.y;
            if (!InBorder(toX, toY) || !HasTile(toX, toY)) return false;
            var fromTile = _tiles[fromX, fromY];
            var toTile = _tiles[toX, toY];
            if (toTile.IsBlocked) return false;
            fromTile.OnCharacterExit(character);
            toTile.OnCharacterEnter(character);
            character.transform.position = to;
            return true;
        }
    }
}