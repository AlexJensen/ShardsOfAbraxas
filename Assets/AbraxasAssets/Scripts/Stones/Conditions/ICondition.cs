using Abraxas.Events.Managers;
using Abraxas.Stones.Controllers;

namespace Abraxas.Stones.Conditions
{
    public interface ICondition
    {
        void Initialize(IStoneController stoneController, ICondition condition);
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
        bool IsMet();
        void Construct(IEventManager eventManager);
    }
}