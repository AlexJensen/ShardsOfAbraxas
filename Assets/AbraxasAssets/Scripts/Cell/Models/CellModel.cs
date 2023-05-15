using Abraxas.Cards.Controllers;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Cells.Models
{
    class CellModel : ICellModel
    {
        #region Fields
        readonly List<ICardController> _cards = new();
        Point _fieldPosition;
        #endregion

        #region Methods
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
