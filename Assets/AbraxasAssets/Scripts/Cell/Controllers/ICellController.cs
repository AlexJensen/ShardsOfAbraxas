using Abraxas.Cards.Controllers;
using Abraxas.Cells.Models;
using Abraxas.Cells.Views;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Cells.Controllers
{
    public interface ICellController
    {
        ICellView View { get; }
        List<ICardController> Cards { get; }
        Players.Players Player { get; }
        Point FieldPosition { set; get; }

        void AddCard(ICardController card);
        void Initialize(ICellView view, ICellModel model);
        void RemoveCard(ICardController card);
    }
}
