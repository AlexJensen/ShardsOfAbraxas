using Abraxas.Events.Managers;
using Abraxas.Stones.Controllers;
using Zenject;

namespace Abraxas.Stones.Conditions
{
    public interface ICondition
    {
        void Initialize(IStoneController stoneController, ICondition condition, DiContainer container);
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
        bool IsMet();
    }
}