using Abraxas.Zones.Views;

namespace Abraxas.Zones.Hands.Views
{
    interface IHandView : IZoneView
    {
        int CardPlaceholderSiblingIndex { get; }
    }
}
