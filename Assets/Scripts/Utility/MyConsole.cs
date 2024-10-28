using UnityEngine;

namespace ConsoleSystem
{
    public enum MessageColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        White,
        Cyan,
        Magenta,
        Gray,
        Black,
        Orange,
        Purple,
        Brown,
    }
    
    public static class MyConsole
    {
        // 指定颜色输出
        public static void Print(string message, MessageColor color = MessageColor.White)
        {
#if UNITY_EDITOR
            Debug.Log($"<color={color}>{message}</color>");
#endif
        }
    }
}