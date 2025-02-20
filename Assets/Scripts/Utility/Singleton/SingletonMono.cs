using UnityEngine;

namespace Utility.Singleton
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object InstanceLock = new();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
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

        // ReSharper disable once StaticMemberInGenericType
        private static bool _isInitialized;
        private void Awake()
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
            
            if (_isInitialized) return;
            _isInitialized = true;
            OnAwake();
        }

        /// <summary>
        /// 多个实例时，只有第一个实例会调用OnAwake方法
        /// </summary>
        protected virtual void OnAwake() { }
    }
}