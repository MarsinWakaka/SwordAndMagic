using System;
using Configuration;
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
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        public virtual void OnPause()
        {
            _canvasGroup.blocksRaycasts = false;
        }

        public virtual void OnResume()
        {
            _canvasGroup.blocksRaycasts = true;
        }

        public virtual void OnExit()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}