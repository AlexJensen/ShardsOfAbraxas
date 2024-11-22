using Abraxas.Events.Managers;
using Abraxas.Stones.Controllers;
using Zenject;

namespace Abraxas.Stones.Conditions
{
    public interface ICondition
    {
        void Initialize(IStoneController stoneController, DiContainer container);
        bool IsMet();
    }
}