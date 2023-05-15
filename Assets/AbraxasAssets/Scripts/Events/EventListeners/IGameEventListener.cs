using System.Collections;

namespace Abraxas.Events
{
    public interface IGameEventListener<T>
    {
        IEnumerator OnEventRaised(T eventData);
    }
}
