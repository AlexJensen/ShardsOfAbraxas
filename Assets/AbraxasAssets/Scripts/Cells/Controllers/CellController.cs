using Abraxas.Cards.Controllers;
using Abraxas.Cells.Models;
using Abraxas.Cells.Views;
using System.Drawing;
using UnityEngine;
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

            _model.Player = _view.Player;
        }

        public class Factory : PlaceholderFactory<ICellView, ICellController>
        {

        }
        #endregion

        #region Properties
        public Player Player { get => _model.Player; }
        public Point FieldPosition
        {
            get => _model.FieldPosition; 
            set
            {
                _model.FieldPosition = value;
            }
        }
        public int CardsOnCell
        {
            get
            {
                return _model.CardsOnCell;
            }
        }

        public RectTransform RectTransform => _view.RectTransform;
        #endregion

        #region Methods
        public void AddCard(ICardController card)
        {
            _model.AddCard(card);
            _view.SetChild(card.TransformManipulator.Transform);
            card.Cell = this;
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
