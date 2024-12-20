using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;


namespace Abraxas.Zones.Fields.Managers
{
    public interface IFieldManager
    {
        void RemoveCard(ICardController card);
        IEnumerator MoveCardToCell(ICardController card, Point fieldPosition);
        PointF GetCellDimensions();
        IEnumerator StartCombat();
        void SetField(IFieldController fieldController);
        void AddCard(ICardController card);
        void HighlightPlayableOpenCells(ICardController cardController);
        void SetHighlightVisible(bool val);
        ICellController[] GetOpenCells(Players.Players player);
        List<ICellController> GetRow(int y);
    }
}
