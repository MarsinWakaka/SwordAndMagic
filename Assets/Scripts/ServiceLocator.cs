using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once ClassNeverInstantiated.Global
public class ServiceLocator
{
    private static readonly Dictionary<Type, object> Services = new();

    public static void Register<T>(T service) where T : class
    {
        if (Services.ContainsKey(typeof(T)))
        {
            Debug.LogError($"Service already registered: {typeof(T)}");
            return;
        }
        Services[typeof(T)] = service;
    }
    
    public static void Unregister<T>() where T : class
    {
        if (Services.ContainsKey(typeof(T)))
        {
            Services.Remove(typeof(T));
            return;
        }
        Debug.LogWarning($"Service not not in Dict: {typeof(T)}");
    }

    public static T Get<T>() where T : class
    {
        if (Services.TryGetValue(typeof(T), out var service))
        {
            return service as T;
        }
        Debug.LogError($"Service not found: {typeof(T)}");
        return null;
    }
}