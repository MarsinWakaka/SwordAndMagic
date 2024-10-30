using System;
using System.Collections.Generic;
using BattleSystem.FactionSystem;
using GamePlaySystem.FactionSystem;
using ResourcesSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entity
{
    public interface IEntityFactory
    {
        public Unit.Character CreateCharacter(int entityID, Vector2 position, FactionType factionType);
        public void CreateCharacter(int entityID, Action<Unit.Character> actionOnCreate);
        public Tile CreateTile(TileType tileType, Vector2 position);
    }
    
    public class EntityFactoryImpl : IEntityFactory 
    {
        private readonly Dictionary<int, Unit.Character> _characterDict = new();
        // private readonly Dictionary<int, Tile> _tileDict = new();
        private Tile _tilePrefab;
        
        public EntityFactoryImpl(string characterPrefabPath, string tilePrefabPath)
        {
            IResourceManager resourceManager = ServiceLocator.Get<IResourceManager>();
            resourceManager.LoadAllResourcesAsyncByTag<GameObject>(characterPrefabPath, (gos) =>
            {
                foreach (var go in gos)
                {
                    if (go.TryGetComponent<Unit.Character>(out var character))
                    {
                        _characterDict.Add(character.entityID, character);
                    }
                }
            });
            resourceManager.LoadResourceAsync<GameObject>(tilePrefabPath, (go) =>
            {
                if (go.TryGetComponent<Tile>(out var character))
                {
                    _tilePrefab = character;
                }
            });
        }

        public Unit.Character CreateCharacter(int entityID, Vector2 position, FactionType factionType)
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

        public void CreateCharacter(int entityID, Action<Unit.Character> actionOnCreate)
        {
            if (_characterDict.TryGetValue(entityID, out var character))
            {
                var newCharacter = Object.Instantiate(character);
                actionOnCreate?.Invoke(newCharacter);
                return;
            }
            Debug.LogError("实体不存在: " + entityID);
        }

        public Tile CreateTile(TileType tileType, Vector2 position)
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