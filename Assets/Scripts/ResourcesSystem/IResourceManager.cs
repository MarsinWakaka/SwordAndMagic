using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ResourcesSystem
{
    public interface IResourceManager
    {
        public T LoadResource<T>(string resURL) where T : Object;
        public void LoadResourceAsync<T>(string resURL, Action<T> callback) where T : Object;
        public void LoadAllResourcesAsync<T>(string resURL, Action<List<T>> callback) where T : Object;
        public void UnloadResource(string resURL);
        public void ClearCache();
    }
    
    public class AddressableManager : IResourceManager
    {
        // 缓存已加载资源的字典
        private readonly Dictionary<string, object> _resourceCache = new();

        // 加载资源
        public T LoadResource<T>(string resURL) where T : Object
        {
            throw new NotImplementedException("AddressableManager 不支持同步加载资源");
        }

        // 异步加载资源
        public void LoadResourceAsync<T>(string resURL, Action<T> callback) where T : Object
        {
            // 检查缓存中是否存在资源
            if (_resourceCache.TryGetValue(resURL, out var cachedResource))
            {
                callback?.Invoke(cachedResource as T);
                return;
            }

            // 异步加载资源
            var handle = Addressables.LoadAssetAsync<T>(resURL);
            handle.Completed += operation =>
            {
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    // 缓存加载的资源
                    _resourceCache[resURL] = handle.Result;
                    callback?.Invoke(handle.Result);
                }
                else
                {
                    Debug.LogError($"Failed to load resource from {resURL}: {operation.OperationException}");
                    callback?.Invoke(null);
                }
            };
        }
        
        // 加载指定路径下的所有资源
        public void LoadAllResourcesAsync<T>(string folderAddress, Action<List<T>> callback) where T : Object
        {
            var handle = Addressables.LoadResourceLocationsAsync(folderAddress);
            handle.Completed += operation =>
            {
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    var resources = new List<T>();
                    int totalResources = operation.Result.Count; // 总资源数量
                    int loadedCount = 0; // 已加载的资源数量
                    // IResourceLocation是资源的地址信息，包含资源的主键、路径、依赖等等，官方描述：此变量包含足够信息去加载资源
                    foreach (IResourceLocation location in operation.Result)
                    {
                        Addressables.LoadAssetAsync<T>(location).Completed += loadOperation =>
                        {
                            if (loadOperation.Status == AsyncOperationStatus.Succeeded)
                            {
                                // _resourceCache[location.PrimaryKey] = handle.Result; //应用缓存之前，需要先处理资源的释放
                                resources.Add(loadOperation.Result);
                            }
                            else
                            {
                                Debug.LogError($"资源加载失败：{location.PrimaryKey}: {loadOperation.OperationException}");
                            }

                            // 检查是否所有资源都已处理完
                            loadedCount++;
                            if (loadedCount == totalResources)
                            {
                                callback?.Invoke(resources); // 所有资源处理完后调用回调
                            }
                        };
                    }
                }
            };
        }
        
        // 卸载资源
        public void UnloadResource(string resURL)
        {
            if (_resourceCache.ContainsKey(resURL))
            {
                // 从缓存中移除并释放资源
                var resource = _resourceCache[resURL];
                Addressables.Release(resource);
                _resourceCache.Remove(resURL);
            }
            else
            {
                Debug.LogWarning($"Resource {resURL} not found in cache.");
            }
        }

        // 清空所有缓存资源
        public void ClearCache()
        {
            foreach (var key in _resourceCache.Keys)
            {
                Addressables.Release(_resourceCache[key]);
            }
            _resourceCache.Clear();
        }
    }
}