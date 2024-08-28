using Abraxas.Stones.Controllers;

namespace Abraxas.Stones.Targets
{
    public interface ITarget
    {
        void Initialize(IStoneController stoneController);
    }
}
