using UnityEngine;
using UnityEngine.Serialization;

namespace BattleSystem.Style
{
    public class SelectStyle : MonoBehaviour
    {
        public SpriteRenderer numRenderer;
        
        public void SetNumSprite(Sprite sprite)
        {
            numRenderer.sprite = sprite;
        }
    }
}