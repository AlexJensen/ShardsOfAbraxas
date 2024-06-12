using Abraxas.Events.Managers;
using System;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    public interface ICondition
    {
        void Initialize(TriggerStone stoneController, ICondition condition);
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
        bool IsMet();
        void Construct(IEventManager eventManager);
    }
}