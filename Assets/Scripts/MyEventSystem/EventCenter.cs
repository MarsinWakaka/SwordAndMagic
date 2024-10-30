using System;
using System.Collections.Generic;
using Utility;

public class IEventInterface
{
    
}

public class EventHandler : IEventInterface
{
    private event Action Event;

    public void AddListener(Action action)
    {
        Event += action;
    }

    public void RemoveListener(Action action)
    {
        Event -= action;
    }
    
    public void Invoke()
    {
        Event?.Invoke();
    }
    
    public bool HasNoListeners() => Event == null;
}

public class EventHandler<T> : IEventInterface
{
    private event Action<T> Event;

    public void AddListener(Action<T> action)
    {
        Event += action;
    }

    public void RemoveListener(Action<T> action)
    {
        Event -= action;
    }
    
    public void Invoke(T arg)
    {
        Event?.Invoke(arg);
    }
    
    public bool HasNoListeners() => Event == null;
}

public class EventHandler<T1, T2> : IEventInterface
{
    private event Action<T1, T2> Event;

    public void AddListener(Action<T1, T2> action)
    {
        Event += action;
    }

    public void RemoveListener(Action<T1, T2> action)
    {
        Event -= action;
    }
    
    public void Invoke(T1 arg1, T2 arg2)
    {
        Event?.Invoke(arg1, arg2);
    }
    
    // TODO 提供清空事件的函数？

    public bool HasNoListeners() => Event == null;
}

public class EventHandler<T1, T2, T3> : IEventInterface
{
    private event Action<T1, T2, T3> Event;

    public void AddListener(Action<T1, T2, T3> action)
    {
        Event += action;
    }

    public void RemoveListener(Action<T1, T2, T3> action)
    {
        Event -= action;
    }
    
    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        Event?.Invoke(arg1, arg2, arg3);
    }
    
    // TODO 提供清空事件的函数？

    public bool HasNoListeners() => Event == null;
}
public class EventCenter<TK> : Singleton<EventCenter<TK>>
{
    private readonly Dictionary<TK, IEventInterface> _events = new();
    
    public void Clear() {
        _events.Clear();
    }

    #region 参数个数为0
    
    public void AddListener(TK eventName, Action action)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events.Add(eventName, new EventHandler());
        }
        
        var eventHandler = _events[eventName] as EventHandler;
        eventHandler?.AddListener(action);
    }
    
    public void RemoveListener(TK eventName, Action action)
    {
        if (_events.TryGetValue(eventName, out var @event))
        {
            var eventHandler = @event as EventHandler;
            eventHandler?.RemoveListener(action);
            
            // 如果 eventHandler 没有任何监听者则移除
            if (eventHandler != null && eventHandler.HasNoListeners())
                _events.Remove(eventName);
        }
    }

    public void Invoke(TK eventName)
    {
        if (_events.TryGetValue(eventName, out var @event))
        {
            var eventHandler = @event as EventHandler;
            eventHandler?.Invoke();
        }
    }
    
    #endregion
    
    #region 参数个数为1
    
    public void AddListener<T1>(TK eventName, Action<T1> action)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events.Add(eventName, new EventHandler<T1>());
        }
        
        var eventHandler = _events[eventName] as EventHandler<T1>;
        eventHandler?.AddListener(action);
    }
    
    public void RemoveListener<T1>(TK eventName, Action<T1> action)
    {
        if (_events.TryGetValue(eventName, out var @event))
        {
            var eventHandler = @event as EventHandler<T1>;
            eventHandler?.RemoveListener(action);
            
            // 如果 eventHandler 没有任何监听者则移除
            if (eventHandler != null && eventHandler.HasNoListeners())
                _events.Remove(eventName);
        }
    }
    
    public void Invoke<T1>(TK eventName, T1 arg)
    {
        if (_events.TryGetValue(eventName, out var @event))
        {
            var eventHandler = @event as EventHandler<T1>;
            eventHandler?.Invoke(arg);
        }
    }

    #endregion
    
    #region 参数个数为2
    
    public void AddListener<T1, T2>(TK eventName, Action<T1, T2> action)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events.Add(eventName, new EventHandler<T1, T2>());
        }
        
        var eventHandler = _events[eventName] as EventHandler<T1, T2>;
        eventHandler?.AddListener(action);
    }

    public void RemoveListener<T1, T2>(TK eventName, Action<T1, T2> action)
    {
        if (_events.TryGetValue(eventName, out var @event))
        {
            var eventHandler = @event as EventHandler<T1, T2>;
            eventHandler?.RemoveListener(action);
            
            // 如果 eventHandler 没有任何监听者则移除
            if (eventHandler != null && eventHandler.HasNoListeners())
                _events.Remove(eventName);
        }
    }
    
    public void Invoke<T1, T2>(TK eventName, T1 arg1, T2 arg2)
    {
        if (_events.TryGetValue(eventName, out var @event))
        {
            var eventHandler = @event as EventHandler<T1, T2>;
            eventHandler?.Invoke(arg1, arg2);
        }
    }
    
    #endregion

    #region 参数个数为3

    public void AddListener<T1, T2, T3>(TK eventName, Action<T1, T2, T3> action)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events.Add(eventName, new EventHandler<T1, T2, T3>());
        }
        
        var eventHandler = _events[eventName] as EventHandler<T1, T2, T3>;
        eventHandler?.AddListener(action);
    }

    public void RemoveListener<T1, T2, T3>(TK eventName, Action<T1, T2, T3> action)
    {
        if (_events.TryGetValue(eventName, out var @event))
        {
            var eventHandler = @event as EventHandler<T1, T2, T3>;
            eventHandler?.RemoveListener(action);
            
            // 如果 eventHandler 没有任何监听者则移除
            if (eventHandler != null && eventHandler.HasNoListeners())
                _events.Remove(eventName);
        }
    }
    
    public void Invoke<T1, T2, T3>(TK eventName, T1 arg1, T2 arg2, T3 arg3)
    {
        if (_events.TryGetValue(eventName, out var @event))
        {
            var eventHandler = @event as EventHandler<T1, T2, T3>;
            eventHandler?.Invoke(arg1, arg2, arg3);
        }
    }

    #endregion
}