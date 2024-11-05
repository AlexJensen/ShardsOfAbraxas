using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Zones.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Zones.Fields.Controllers
{
	public interface IFieldController : IZoneController
    {
        List<List<ICellController>> FieldGrid { get; }
        IEnumerator StartCombat();
        IEnumerator MoveCardToCell(ICardController card, Point fieldPos);
        PointF GetDefaultCellDimensions();
        void HighlightPlayableOpenCells(ICardController cardController);
        void SetHighlightVisible(bool val);
        ICellController[] GetOpenCells(Players.Players player);
        IEnumerator MoveCardToCell(ICardController card, ICellController cell);
    }
}
