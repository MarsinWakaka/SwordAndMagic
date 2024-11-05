using DG.Tweening;
using Entity;
using MyEventSystem;
using UnityEngine;

namespace GamePlaySystem.BattleFXSystem.Helper
{
    public class HoverStyleHelper : MonoBehaviour
    {
        [Header("Style Settings")]
        public Transform hoverStyle;
        public float hoverDuration;
        private const float HoverFadeDuration = 0.25f;

        #region Cache Variable
        private SpriteRenderer _hoverSpriteRenderer;
        #endregion

        private void Awake()
        {
            _hoverSpriteRenderer = hoverStyle.GetComponent<SpriteRenderer>();
            EventCenter<GameEvent>.Instance.AddListener<BaseEntity>(GameEvent.SetHoverEntity, SetHoverFX);
        }

        private bool _isCancelingHover;
        
        private void SetHoverFX(BaseEntity baseEntity)
        {
            if (baseEntity == null) {
                CancelHoverFX();
            }
            else
            {
                SetHoverFX(baseEntity.transform.position);
            }
        }
        
        public void SetHoverFX(Vector3 position)
        {
            if (_isCancelingHover)
            {
                hoverStyle.DOKill();
                _hoverSpriteRenderer.DOFade(1, HoverFadeDuration);
            }

            hoverStyle.DOMove(position, hoverDuration);
            _isCancelingHover = false;
        }
        public void CancelHoverFX()
        {
            _hoverSpriteRenderer.DOFade(0, HoverFadeDuration);
            _isCancelingHover = true;
        }
    }
}