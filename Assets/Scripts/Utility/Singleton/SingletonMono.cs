using UnityEngine;

namespace Utility.Singleton
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                // 1. If the instance is not null, return the instance.
                if (_instance != null) return _instance;
                
                // 2. If the instance is null, find the instance in the scene.
                _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                
                // 3. If the instance is still null, create a new instance.
                var obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);

                return _instance;
            }
        }
        
        public static bool IsInstanceNull => _instance == null;
        
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}