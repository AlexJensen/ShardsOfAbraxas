using Abraxas.Cells.Controllers;
using System.Collections;

namespace Abraxas.Cards.Controllers
{
    public interface ICardDragHandler
    {
        void OnBeginDrag();
        IEnumerator OnCardDraggedOverCell(ICellController cell);
        void OnDrag();
        IEnumerator ReturnFromOverlayToHand();
    }
}
