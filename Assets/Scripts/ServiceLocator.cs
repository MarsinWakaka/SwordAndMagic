using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once ClassNeverInstantiated.Global
public class ServiceLocator
{
    private static readonly Dictionary<Type, object> Services = new();

    public static void Register<T>(T service) where T : class
    {
        Services[typeof(T)] = service;
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