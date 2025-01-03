using Abraxas.Core;

namespace Abraxas.VFX.Managers
{
    public interface IVFXManager : IManager
    {
        float CurrentFlickerOpacity { get; }
    }
}
