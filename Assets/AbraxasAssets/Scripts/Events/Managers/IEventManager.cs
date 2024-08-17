
using System;
using System.Collections;

namespace Abraxas.Events.Managers
{
    public interface IEventManager
    {
        void AddListener<T>(IGameEventListener<T> listener);
        IEnumerator RaiseEvent<T>(T eventData);
        void RemoveListener<T>(IGameEventListener<T> listener);
    }
}
