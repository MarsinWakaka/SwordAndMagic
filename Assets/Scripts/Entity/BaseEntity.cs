using System;
using BattleSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity
{
    [Flags]
    public enum EntityType
    {
        Character = 1, // 如果是角色，则判断角色阵营
        Item = 2,
        Tile = 4,
    }

    public interface IInvestigation
    {
        // string GetDescription();
    }
    
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class BaseEntity : MonoBehaviour, IInvestigation
    {
        [Header("实体属性")]
        public int entityClassID;
        public Guid EntityID;
        // 只读属性
        public Sprite sprite;
        public EntityType entityType;

        protected SpriteRenderer Renderer;
        
        protected void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
            Renderer.sprite = sprite;
        }
        
        public void Initialize(Guid entityID, Vector2 position)
        {
            EntityID = entityID;
            transform.position = position;
        }

        protected virtual void OnDestroy()
        {
            // 战斗管理器先一步销毁
            if (CharacterManager.IsInstanceNull) return;
        }

        // public abstract string GetDescription();
    }
}