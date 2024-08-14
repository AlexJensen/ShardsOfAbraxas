using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abraxas.Events.Managers
{
    public class EventManager : MonoBehaviour, IEventManager
    {
        private readonly Dictionary<Type, List<IGameEventListenerBase>> _eventListeners = new();

        public void AddListener<T>(Type type, IGameEventListener<T> listener)
        {
            Type eventType = typeof(T);
            if (!_eventListeners.TryGetValue(eventType, out var listeners))
            {
                listeners = new List<IGameEventListenerBase>();
                _eventListeners.Add(eventType, listeners);
            }
            listeners.Add(listener);
        }

        public void RemoveListener<T>(Type type, IGameEventListener<T> listener)
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out var listeners))
            {
                listeners.Remove(listener);
            }
        }

        public IEnumerator RaiseEvent<T>(Type type, T eventData)
        {
            if (_eventListeners.TryGetValue(type, out var listeners))
            {
                var castListeners = listeners.Cast<IGameEventListener<T>>();
                var listenersCopy = new List<IGameEventListener<T>>(castListeners);
                foreach (var routine in from IGameEventListener<T> listener in listenersCopy
                                        where listener.ShouldReceiveEvent(eventData)
                                        let routine = listener.OnEventRaised(eventData)
                                        select routine)
                {
                    while (routine.MoveNext())
                    {
                        object current = routine.Current;
                        if (current != null)
                        {
                            yield return current;
                        }
                    }
                }
            }
        }
    }

    public interface IGameEventListenerBase { }
}