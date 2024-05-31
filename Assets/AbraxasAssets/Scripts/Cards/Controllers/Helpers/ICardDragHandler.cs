using Abraxas.Cells.Controllers;
using System.Collections;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// ICardsDragHandler is an interface for handling card dragging events.
    /// </summary>
    public interface ICardDragHandler
    {
        void OnBeginDrag();
        IEnumerator OnCardDraggedOverCell(ICellController cell);
        void OnDrag();
        void OnEndDrag();
        IEnumerator ReturnFromOverlayToHand();

    }
}
