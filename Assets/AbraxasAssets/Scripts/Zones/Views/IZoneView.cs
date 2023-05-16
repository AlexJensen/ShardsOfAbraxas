using Abraxas.Cards.Views;
using System.Collections;

namespace Abraxas.Zones.Views
{
    public interface IZoneView
    {
        Players.Players Player { get; }

        IEnumerator MoveCardToZone(ICardView card, int index = 0);
    }
}
