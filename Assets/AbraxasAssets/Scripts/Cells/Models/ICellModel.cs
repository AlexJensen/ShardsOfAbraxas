using Abraxas.Cards.Controllers;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cells.Models
{
    public interface ICellModel
    {
        Player Player { get; set; }
        Point FieldPosition { get; set; }
        int CardsOnCell { get; }

        void AddCard(ICardController card, int index = 0);
        ICardController GetCardAtIndex(int index);
        void RemoveCard(ICardController card);
    }
}
