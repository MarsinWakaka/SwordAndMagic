using DG.Tweening;
using UnityEngine;

namespace BattleSystem.BattleFXSystem.Helper
{
    public class HoverStyleHelper : MonoBehaviour
    {
        [Header("Style Settings")]
        public Transform hoverStyle;
        public float hoverDuration;
        public float hoverFadeDuration;

        #region Cache Variable
        private SpriteRenderer _hoverSpriteRenderer;
        #endregion

        private void Awake()
        {
            _hoverSpriteRenderer = hoverStyle.GetComponent<SpriteRenderer>();
        }

        private bool _isCancelingHover;
        public void SetHoverFX(Vector2 position)
        {
            if (_isCancelingHover)
            {
                hoverStyle.DOKill(_hoverSpriteRenderer);
                _hoverSpriteRenderer.DOFade(1, hoverFadeDuration);
            }

            hoverStyle.DOMove(position, hoverDuration);
            _isCancelingHover = false;
        }
        public void CancelHoverFX()
        {
            _hoverSpriteRenderer.DOFade(0, hoverFadeDuration);
            _isCancelingHover = true;
        }
    }
}