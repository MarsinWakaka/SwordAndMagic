using System;
using System.Collections.Generic;
using GamePlaySystem;
using GamePlaySystem.FactionSystem;
using ResourcesSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entity
{
    public interface IEntityFactory
    {
        public Character CreateCharacter(int entityID, Vector3 position, FactionType factionType);
        public void CreateCharacter(int entityID, Action<Character> actionOnCreate);
        public Tile CreateTile(TileType tileType, Vector3 position);
    }
    
    public class EntityFactoryImpl : IEntityFactory 
    {
        private readonly Dictionary<int, Character> _characterDict = new();
        private readonly string characterPrefabPath;
        private readonly string tilePrefabPath;
        private Tile _tilePrefab;
        
        public EntityFactoryImpl(string characterPrefabPath, string tilePrefabPath)
        {
            this.characterPrefabPath = characterPrefabPath;
            this.tilePrefabPath = tilePrefabPath;
        }
        
        public void LoadEntityPrefab(Action onComplete)
        {
            bool isCharacterLoaded = false;
            bool isTileLoaded = false;
            // 加载实体数据
            IResourceManager resourceManager = ServiceLocator.Get<IResourceManager>();
            resourceManager.LoadAllResourcesAsyncByTag<GameObject>(characterPrefabPath, (gos) =>
            {
                foreach (var go in gos)
                {
                    if (go.TryGetComponent<Character>(out var character))
                    {
                        _characterDict.Add(character.entityID, character);
                    }
                }
                isCharacterLoaded = true;
                if (isTileLoaded) onComplete?.Invoke();
            });
            resourceManager.LoadResourceAsync<GameObject>(tilePrefabPath, (go) =>
            {
                if (go.TryGetComponent<Tile>(out var tile))
                {
                    if (tile == null) Debug.LogError($"Tile prefab is null: {tilePrefabPath}");
                    _tilePrefab = tile;
                }
                isTileLoaded = true;
                if (isCharacterLoaded) onComplete?.Invoke();
            });
        }

        public Character CreateCharacter(int entityID, Vector3 position, FactionType factionType)
        {
            if (_characterDict.TryGetValue(entityID, out var character))
            {
                var newCharacter = Object.Instantiate(character);
                newCharacter.Initialize(GenerateUniqueID(), position, factionType);
                return newCharacter;
            }
            Debug.LogError("实体不存在: " + entityID);
            return null;
        }

        public void CreateCharacter(int entityID, Action<Character> actionOnCreate)
        {
            if (_characterDict.TryGetValue(entityID, out var character))
            {
                var newCharacter = Object.Instantiate(character);
                actionOnCreate?.Invoke(newCharacter);
                return;
            }
            Debug.LogError("实体不存在: " + entityID);
        }

        public Tile CreateTile(TileType tileType, Vector3 position)
        {
            var newTile = Object.Instantiate(_tilePrefab);
            newTile.Initialize(tileType, position);
            return newTile;
        }

        private Guid GenerateUniqueID()
        {
            return Guid.NewGuid();
        }
    }
}