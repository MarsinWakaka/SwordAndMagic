using Entity;
using MyEventSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputSystem
{
    public class PlayerInputProvider : MonoBehaviour
    {
        [Header("射线相机")]
        [SerializeField] private Camera eventCamera;
        
        // public Action<BaseEntity> onEntityClicked;
        // public Action<BaseEntity> onEntityCancelSelected;
        // public Action OnRightMouseClick;
        private bool isLeftMousePressed;
        private bool isRightMousePressed;
        
        private void ResetInput()
        {
            isLeftMousePressed = false;
            isRightMousePressed = false;
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) 
                return;
            if (TryGetEntityAtMousePosition(out var entity))
            {
                ResetInput();
                isLeftMousePressed = Input.GetMouseButtonDown(0);
            
                if (isLeftMousePressed)
                {
                    EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnEntityLeftClicked, entity);
                }
            }
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.SetHoverEntity, entity);
            isRightMousePressed = Input.GetMouseButtonDown(1);
            if (isRightMousePressed)
            {
                EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnRightMouseClick);
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