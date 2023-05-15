using Abraxas.Cards.Views;
using System.Collections;

namespace Abraxas.Zones.Views
{
    public interface IZoneView
    {
        IEnumerator MoveCardToZone(ICardView card, int index = 0);
    }
}
