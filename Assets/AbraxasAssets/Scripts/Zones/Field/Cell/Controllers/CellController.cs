using Abraxas.Cards.Controllers;
using System.Collections.Generic;
using System.Drawing;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Fields
{
    class CellController : ICellController
    {
        #region Dependencies
        readonly ICellView _view;
        readonly Player _player;
        public CellController(ICellView view, Player player)
        {
            _view = view;
            _player = player;
        }
        #endregion

        #region Fields
        readonly List<ICardController> _cards = new();
        Point _fieldPosition;
        #endregion

        #region Properties
        public ICellView View => _view;
        public List<ICardController> Cards { get => _cards; }
        public Player Player { get => _player; }
        public Point FieldPosition { get => _fieldPosition; set => _fieldPosition = value; }
        #endregion

        #region Methods
        public void AddCard(ICardController card)
        {
            Cards.Add(card);
            View.SetChild(card.View.Transform);
            card.FieldPosition = FieldPosition;
        }

        public void RemoveCard(ICardController card)
        {
            Cards.Remove(card);
        }
        #endregion
    }
}
