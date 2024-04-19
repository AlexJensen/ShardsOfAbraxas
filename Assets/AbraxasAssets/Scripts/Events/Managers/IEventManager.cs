
using System;
using System.Collections;

namespace Abraxas.Events.Managers
{
    public interface IEventManager
    {
        void AddListener<T>(Type type, IGameEventListener<T> listener);
        IEnumerator RaiseEvent<T>(Type type, T eventData);
        void RemoveListener<T>(Type type, IGameEventListener<T> listener);
    }
}
