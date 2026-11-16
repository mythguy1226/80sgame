using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayEvents
{
public static class GameEventBus
{
    private static readonly Dictionary<Type, List<object>> _listeners = new();

    public static void Reset()
    {
        _listeners.Clear();
    }

    public static int ListenerCount<T>() where T : IGameplayMessage
    {
        Type t = typeof(T);
        if(!_listeners.TryGetValue(t, out var list))
        {
            return 0;
        }

        return list.Count;
    }

    public static void Register<T>(object listener) where T : IGameplayMessage
    {
        Type t = typeof(T);
            if (!_listeners.TryGetValue(t, out var list))
            {
            list = new List<object>();
            _listeners[t] = list;
        }

        if (!list.Contains(listener))
        {
            list.Add(listener);
        }
    }

        public static void Unregister<T>(object listener) where T : IGameplayMessage
    {
        Type type = typeof(T);
            if (_listeners.TryGetValue(type, out var list))
            {
            list.Remove(listener);
        }
    }

    public static void Propagate<T>(T gameplayMessage) where T : IGameplayMessage
    {
        Type t = typeof(T);
        if (!_listeners.TryGetValue(t, out var list))
        {
            return;
        }

        object[] listenerSnapshot = list.ToArray();
        for (int i = 0; i < listenerSnapshot.Length; i++){
            if (listenerSnapshot[i] is IGameplayListener<T> listener)
            {
                listener.OnGameplayEvent(gameplayMessage);
            }
        
        }
    }
}
}
