using UnityEngine;

namespace GamePlaySystem.RangeDisplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RangeSlot : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;
        public void SetColor(Color color) => spriteRenderer.color = color;
    }
}