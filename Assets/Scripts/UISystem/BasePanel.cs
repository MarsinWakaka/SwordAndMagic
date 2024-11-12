using DG.Tweening;
using UnityEngine;

namespace UISystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePanel : MonoBehaviour, IPanel
    {
        public PanelType panelType;
        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void OnEnter()
        {
            // transform.SetAsLastSibling(); // 位置管理交由UIManager去做。
            _canvasGroup.DOFade(1, 0.25f);
            transform.DOScale(1, 0.25f).SetEase(Ease.InCubic);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public virtual void OnPause()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public virtual void OnResume()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public virtual void OnExit()
        {
            _canvasGroup.DOFade(0, 0.25f);
            transform.DOScale(0, 0.25f).SetEase(Ease.InCubic);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}