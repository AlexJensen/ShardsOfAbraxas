using Abraxas.Zones.Fields;
using System.Collections;

namespace Abraxas.Cards.Controllers
{
    public interface ICardDragHandler
    {
        void OnBeginDrag();
        void OnCardDraggedOverCell(ICellController cell);
        void OnDrag();
        IEnumerator ReturnFromOverlayToHand();
    }
}
