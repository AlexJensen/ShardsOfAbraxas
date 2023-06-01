
using System;
using System.Collections;

namespace Abraxas.Events.Managers
{
    public interface IEventManager
    {
        void AddListener<T>(Type eventType, IGameEventListener<T> listener);
        IEnumerator RaiseEvent<T>(Type eventType, T eventData);
        void RemoveListener<T>(Type eventType, IGameEventListener<T> listener);
    }
}
