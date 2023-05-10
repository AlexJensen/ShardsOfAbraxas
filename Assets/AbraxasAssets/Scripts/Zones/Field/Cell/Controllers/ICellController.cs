using Abraxas.Cards.Controllers;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Zones.Fields
{
    public interface ICellController
    {
        ICellView View { get; }
        List<ICardController> Cards { get; }
        Players.Players Player { get; }
        Point FieldPosition { set; get; }

        void AddCard(ICardController card);
        void RemoveCard(ICardController card);
    }
}
