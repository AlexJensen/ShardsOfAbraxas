using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Zones.Views;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Zones.Fields.Views
{
    public interface IFieldView : IZoneView
    {
        PointF GetCellDimensions(ICellView cell);
        IEnumerator MoveCardToCell(ICardView card, ICellView cell);
        List<List<ICellController>> GenerateField();
    }
}
