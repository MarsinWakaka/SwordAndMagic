namespace Utility
{
    public class Singleton<T> where T : new()
    {
        static Singleton() { }
        protected Singleton() { }
        public static T Instance { get; } = new T();
        public static bool IsInstanceNull => Instance == null;
    }
}