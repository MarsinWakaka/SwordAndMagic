using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace ResourcesSystem
{
    public interface IResourceManager
    {
        public T LoadResource<T>(string resURL) where T : Object;
        public void LoadResourceAsync<T>(string resURL, Action<T> callback) where T : Object;
        public void LoadAllResourcesAsyncByTag<T>(string resTag, Action<List<T>> callback) where T : Object;
        public void UnloadResource(string resURL);
        public void ClearCache();
    }
}