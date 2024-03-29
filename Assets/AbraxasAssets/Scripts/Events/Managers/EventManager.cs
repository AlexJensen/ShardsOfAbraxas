using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

namespace Abraxas.Events.Managers
{
    public class EventManager : NetworkBehaviour, IEventManager
    {
        #region Fields
        readonly Dictionary<Type, HashSet<object>> _eventListeners = new();
        #endregion

        #region Methods
        public void AddListener<T>(Type eventType, IGameEventListener<T> listener)
        {
            if (!_eventListeners.TryGetValue(eventType, out var listeners))
            {
                listeners = new HashSet<object>();
                _eventListeners[eventType] = listeners;
            }
            listeners.Add(listener);
        }

        public void RemoveListener<T>(Type eventType, IGameEventListener<T> listener)
        {
            if (_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType].Remove(listener);
            }
        }

        public IEnumerator RaiseEvent<T>(Type eventType, T eventData)
        {
            if (_eventListeners.TryGetValue(eventType, out var listeners))
            {
                var listenersCopy = new HashSet<object>(listeners);
                foreach (var listener in listenersCopy)
                {
                    if (listener is IGameEventListener<T> gameEventListener)
                    {
                        yield return gameEventListener.OnEventRaised(eventData);
                    }
                }
            }
        }
        #endregion
    }
}