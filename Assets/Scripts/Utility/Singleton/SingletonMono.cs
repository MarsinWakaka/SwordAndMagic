using UnityEngine;

namespace Utility.Singleton
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _isInitialized;
        private static T _instance;
        private static readonly object Lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if (_instance == null)
                        {
                            _instance = FindObjectOfType<T>();
                            if (_instance != null) return _instance;
                            var singletonObj = new GameObject(typeof(T).Name);
                            _instance = singletonObj.AddComponent<T>();
                            DontDestroyOnLoad(singletonObj);
                        }
                    }
                }
                return _instance;
            }
        }

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
            if (_isInitialized) return; // 不要删哦，这个是为了继承者只被初始化一次
            _isInitialized = true;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }
    }
}