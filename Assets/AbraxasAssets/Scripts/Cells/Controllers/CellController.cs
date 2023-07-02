using Abraxas.Cards.Controllers;
using Abraxas.Cells.Models;
using Abraxas.Cells.Views;
using System.Drawing;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cells.Controllers
{
    class CellController : ICellController
    {
        #region Dependencies
        ICellView _view;
        ICellModel _model;
        public ICellView View => _view;
        public void Initialize(ICellView view, ICellModel model)
        {
            _view = view;
            _model = model;

            _model.FieldPosition = _view.FieldPosition;
            _model.Player = _view.Player;
        }

        public class Factory : PlaceholderFactory<ICellView, ICellController>
        {

        }
        #endregion

        #region Properties
        public Player Player { get => _model.Player; }
        public Point FieldPosition { get => _model.FieldPosition; }
        public int CardsOnCell => _model.CardsOnCell;
        #endregion

        #region Methods
        public void AddCard(ICardController card)
        {
            _model.AddCard(card);
            _view.SetChild(card.View.Transform);
            card.FieldPosition = FieldPosition;
        }

        public void RemoveCard(ICardController card)
        {
            _model.RemoveCard(card);
        }

        public ICardController GetCardAtIndex(int index)
        {
            return _model.GetCardAtIndex(index);
        }
        #endregion
    }
}
