using System;
using System.Collections.Generic;
using BattleSystem.FactionSystem;
using Entity.Tiles;
using ResourcesSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entity
{
    public interface IEntityFactory
    {
        public Unit.Character CreateCharacter(int entityID, Vector2 position, FactionType factionType);
        public Unit.Character CreateCharacter(int entityID, Action<Unit.Character> actionOnCreate);
        public Tile CreateTile(int entityID, Vector2 position);
        public Tile CreateTile(int entityID, Action<Tile> actionOnCreate);
    }
    
    public class EntityFactoryImpl : IEntityFactory 
    {
        private readonly Dictionary<int, Unit.Character> _characterDict = new();
        private readonly Dictionary<int, Tile> _tileDict = new();
        
        public EntityFactoryImpl(string characterPrefabPath, string tilePrefabPath)
        {
            IResourceManager resourceManager = ServiceLocator.Get<IResourceManager>();
            resourceManager.LoadAllResourcesAsync<GameObject>(characterPrefabPath, (gos) =>
            {
                foreach (var go in gos)
                {
                    if (go.TryGetComponent<Unit.Character>(out var character))
                    {
                        _characterDict.Add(character.entityID, character);
                    }
                }
            });
            resourceManager.LoadAllResourcesAsync<GameObject>(tilePrefabPath, (gos) =>
            {
                foreach (var go in gos)
                {
                    if (go.TryGetComponent<Tile>(out var tile))
                    {
                        _tileDict.Add(tile.entityID, tile);
                    }
                }
            });
        }

        public Unit.Character CreateCharacter(int entityID, Vector2 position, FactionType factionType)
        {
            if (_characterDict.TryGetValue(entityID, out var character))
            {
                var newCharacter = GameObject.Instantiate(character);
                newCharacter.Initialize(GenerateUniqueID(), position, factionType);
                return newCharacter;
            }
            Debug.LogError("实体不存在: " + entityID);
            return null;
        }

        private Guid GenerateUniqueID()
        {
            return Guid.NewGuid();
        }

        public Unit.Character CreateCharacter(int entityID, Action<Unit.Character> actionOnCreate)
        {
            if (_characterDict.TryGetValue(entityID, out var character))
            {
                var newCharacter = GameObject.Instantiate(character);
                actionOnCreate?.Invoke(newCharacter);
                return newCharacter;
            }
            Debug.LogError("实体不存在: " + entityID);
            return null;
        }

        public Tile CreateTile(int entityID, Vector2 position)
        {
            if (_tileDict.TryGetValue(entityID, out var tile))
            {
                var newTile = GameObject.Instantiate(tile);
                newTile.name = $"tile.name_{position}";
                newTile.transform.position = position;
                return newTile;
            }
            Debug.LogError("实体不存在: " + entityID);
            return null;
        }
        
        public Tile CreateTile(int entityID, Action<Tile> actionOnCreate)
        {
            if (_tileDict.TryGetValue(entityID, out var tile))
            {
                var newTile = GameObject.Instantiate(tile);
                actionOnCreate?.Invoke(newTile);
                return newTile;
            }
            Debug.LogError("实体不存在: " + entityID);
            return null;
        }
    }
}