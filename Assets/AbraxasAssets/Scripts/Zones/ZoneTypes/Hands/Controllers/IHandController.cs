using Abraxas.Zones.Controllers;

namespace Abraxas.Zones.Hands.Controllers
{
    public interface IHandController : IZoneController
    {
        int CardPlaceholderSiblingIndex { get; }
    }
}
