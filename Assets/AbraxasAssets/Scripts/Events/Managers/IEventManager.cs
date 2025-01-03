
using Abraxas.Core;
using System;
using System.Collections;

namespace Abraxas.Events.Managers
{
    public interface IEventManager: IManager
    {
        void AddListener<T>(IGameEventListener<T> listener);
        IEnumerator RaiseEvent<T>(T eventData);
        void RemoveListener<T>(IGameEventListener<T> listener);
    }
}
