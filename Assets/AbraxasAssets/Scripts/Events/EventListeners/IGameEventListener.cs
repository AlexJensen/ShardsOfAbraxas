using Abraxas.Events.Managers;
using System.Collections;

namespace Abraxas.Events
{
    public interface IGameEventListener<T> : IGameEventListenerBase
    {
        IEnumerator OnEventRaised(T eventData);
        bool ShouldReceiveEvent(T eventData);
    }
}
