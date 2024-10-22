using System;
using Entity;
using EventSystem;
using UnityEngine;

namespace InputSystem
{
    public class PlayerInputProvider : MonoBehaviour
    {
        [Header("射线相机")]
        [SerializeField] private Camera eventCamera;
        
        public Action<BaseEntity> onEntityClicked;
        public Action<BaseEntity> onEntityCancelSelected;
        public Action onRightMouseClick;

        // 可考虑使用BindableProperty
        public bool isAllowInput = true;
        
        // private bool _isLeftControlPressed;
        private bool isLeftMousePressed;
        private bool isRightMousePressed;
        
        private void ResetInput()
        {
            // _isLeftControlPressed = false;
            isLeftMousePressed = false;
            isRightMousePressed = false;
        }

        private void Update()
        {
            if (!isAllowInput) return;
            ResetInput();
            isLeftMousePressed = Input.GetMouseButtonDown(0);
            isRightMousePressed = Input.GetMouseButtonDown(1);
            
            if (isLeftMousePressed)
            {
                if (TryGetEntityAtMousePosition(out var entity))
                {
                    // onEntityClicked?.Invoke(entity);
                    EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnEntityLeftClicked, entity);
                }
            }
            else if (isRightMousePressed)
            {
                onRightMouseClick?.Invoke();
                // if (TryGetEntityAtMousePosition(out var entity)) onEntityCancelSelected?.Invoke(entity);
            }
            
            // HandleHover();
        }

        private void HandleHover()
        {
            if (TryGetEntityAtMousePosition(out var entity))
            {
                CursorTargetManager.Instance.SetEntityHover(entity, entity.transform.position);
            }
            else
            {
                CursorTargetManager.Instance.CancelHover();
            }
        }
        
#if UNITY_EDITOR
        [Header("调试设置")]
        [SerializeField] bool isDebug;
#endif
        private bool TryGetEntityAtMousePosition(out BaseEntity baseEntity)
        {
            baseEntity = null;
            var mousePosition = eventCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                baseEntity = hit.collider.GetComponent<BaseEntity>();
#if UNITY_EDITOR
                if (isDebug) Debug.DrawLine(eventCamera.transform.position, hit.point, Color.red, 1);
            }else{
                if (isDebug)
                {
                    Debug.DrawLine(eventCamera.transform.position, new Vector3(mousePosition.x, mousePosition.y, 0), Color.white, 1);
                }
#endif
            }
            return baseEntity != null;
        }
    }
}