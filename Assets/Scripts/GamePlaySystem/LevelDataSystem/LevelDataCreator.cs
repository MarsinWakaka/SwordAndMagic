using System;
using System.Collections.Generic;
using Configuration;
using Entity;
using GamePlaySystem.FactionSystem;
using GamePlaySystem.TileSystem;
using UnityEngine;
using Utility.SerializeTool;

namespace GamePlaySystem.LevelDataSystem
{
    public enum EditorMode
    {
        Tile,
        Unit,
        Item
    }
    
    /// <summary>
    /// 现处于快速Demo开发阶段，功能尚未实现。
    /// </summary>
    public class LevelDataCreator : MonoBehaviour
    {
        [Header("编辑模式")]
        public EditorMode editorMode = EditorMode.Tile;
        
        [Header("基本数据编辑")]
        public LevelData levelData;
        
        [Header("地图数据编辑")]
        public Tile tilePrefab;
        public Character unitPrefab;
        // public GameObject itemPrefab;
        public Transform tileRoot;
        public Transform unitRoot;
        // public Transform itemRoot;
        private readonly Dictionary<int, Tile> tiles = new();
        private readonly Dictionary<int, Character> characters = new();

        private ISerializeTool _serializeTool;
        private EntityFactoryImpl _entityFactory;
        private TileManager _tileManager;
        
        private int _curTileType;
        
        private Camera _camera;
        private void Awake()
        {
            _camera = Camera.main;
            var config = ServiceLocator.Get<IConfigService>().ConfigData;
            _serializeTool = ServiceLocator.Get<ISerializeTool>();
            // TODO 解决部署资源获取问题，从而将服务的注册与卸载代码移到场景切换类中
            _entityFactory = new EntityFactoryImpl(config.characterPrefabPath, config.tilePrefabPath);
            _entityFactory.LoadEntityPrefab(() => {
                Debug.Log("Entity Prefab loaded");
                _tileManager = new TileManager();
                _tileManager.Initialize(config.tileDataPath);
            });
        }

        public void SetTileData(Vector3 pos, TileType tileType)
        {
            if (!tiles.TryGetValue(KeyFromPos((int)pos.x, (int)pos.y), out var tile))
            {
                tile = _entityFactory.CreateTile(tileType, pos);
                tiles.Add(KeyFromPos((int)pos.x, (int)pos.y), tile);
                _tileManager.UpdateTileType(tile, tileType);
            }
            else
            {
                _tileManager.UpdateTileType(tile, tileType);
            }
        }
        
        public void SetUnitData(Vector3 pos, int unitID)
        {
            if (characters.TryGetValue(KeyFromPos((int)pos.x, (int)pos.y), out var character))
            {
                Destroy(character.gameObject);
                characters.Remove(KeyFromPos((int)pos.x, (int)pos.y));
            }
            character = _entityFactory.CreateCharacter(unitID, pos, FactionType.Player);
            characters.Add(KeyFromPos((int)pos.x, (int)pos.y), character);
        }

        private Vector3 MousePosRound(Vector3 mousePos)
        {
            var pos = _camera.ScreenToWorldPoint(mousePos);
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = 0;
            return pos;
        }
        
        private Vector3 gridPos;
        [SerializeField] private Transform gridLocatorHelp;
        public void Update()
        {
            var mousePos = Input.mousePosition;
            var scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");
            gridPos = MousePosRound(mousePos);
            gridLocatorHelp.position = gridPos;
            if (gridPos.x < 0 || gridPos.y < 0) return;
            
            switch (editorMode)
            {
                case EditorMode.Tile:
                    if (scrollInput != 0) {
                        _curTileType = (_curTileType + 3 + scrollInput > 0 ? 1 : -1) % 3;
                        Debug.Log($"TileType: {(TileType)_curTileType}");
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        SetTileData(gridPos, (TileType)_curTileType);
                    }
                    break;
                case EditorMode.Unit:
                    SetUnitData(gridPos, _curTileType);
                    break;
                case EditorMode.Item:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void LoadLevelData()
        {
        }
        
        public void SaveLevelData()
        {
        }

        private static int KeyFromPos(int x, int y) {
            return (x << 10) + y;
        }
    }
}