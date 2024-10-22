using UnityEngine;

namespace Obsolete
{
    public class MouseStyleHelper : MonoBehaviour
    {
        public Texture2D defaultMouseStyle;

        private void Start()
        {
            ResetMouseStyle();
        }

        public void SetMouseStyle(ref Texture2D sprite)
        {
            // 设置鼠标样式
            Cursor.SetCursor(sprite, Vector2.zero, CursorMode.Auto);
        }

        public void ResetMouseStyle()
        {
            Cursor.SetCursor(defaultMouseStyle, Vector2.zero, CursorMode.Auto);
        }
    }
}