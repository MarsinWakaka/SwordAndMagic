using System;
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
    
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class BaseEntity : MonoBehaviour//, IInvestigation
    {
        [FormerlySerializedAs("entityClassID")] [Header("实体属性")]
        public int entityID;
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

        // public abstract string GetDescription();
        // public abstract string GetDescription();
    }
}