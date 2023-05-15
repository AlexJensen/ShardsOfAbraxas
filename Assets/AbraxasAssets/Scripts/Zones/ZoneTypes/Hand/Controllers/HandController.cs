using Abraxas.Zones.Controllers;
using Abraxas.Zones.Hands.Views;

namespace Abraxas.Zones.Hands.Controllers
{
    class HandController : ZoneController, IHandController
    {
        public int CardPlaceholderSiblingIndex => ((IHandView)View).CardPlaceholderSiblingIndex;
    }
}
