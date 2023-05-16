using Abraxas.Cards.Controllers;
using System.Collections.Generic;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cells.Models
{
    class CellModel : ICellModel
    {
        #region Fields
        readonly List<ICardController> _cards = new();
        #endregion

        #region Properties
        public Point FieldPosition { get; set; }
        public Player Player { get; set; }
        public int CardsOnCell => _cards.Count;
        #endregion

        #region Methods
        public ICardController GetCardAtIndex(int index)
        {
            return _cards[index];
        }

        public void AddCard(ICardController card, int index)
        {
            _cards.Insert(index, card);
        }

        public void RemoveCard(ICardController card)
        {
            _cards.Remove(card);
        }
        #endregion
    }
}
