using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Zones.Views;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Zones.Fields.Views
{
    public interface IFieldView : IZoneView
    {
        PointF GetCellDimensions(ICellController cell);
        IEnumerator MoveCardToCell(ICardController card, ICellController cell);
        List<List<ICellController>> GenerateField();
    }
}
