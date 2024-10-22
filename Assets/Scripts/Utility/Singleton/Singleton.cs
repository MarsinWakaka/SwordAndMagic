namespace Utility
{
    public class Singleton<T> where T : new()
    {
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
        
        public static bool IsInstanceNull => _instance == null;
    }
}