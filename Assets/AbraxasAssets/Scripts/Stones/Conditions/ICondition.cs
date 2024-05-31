using Abraxas.Cards.Controllers;
using Abraxas.Events.Managers;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    public interface ICondition<T>
    {
        void Initialize(IStoneController stoneController);
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
        bool IsMet(ICardController card, T eventData);
        void Construct(IEventManager eventManager);
    }
}