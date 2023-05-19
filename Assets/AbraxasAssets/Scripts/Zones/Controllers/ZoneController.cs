using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using Abraxas.Zones.Models;
using Abraxas.Zones.Views;
using System.Collections;
using System.Collections.Generic;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Controllers
{
    class ZoneController :IZoneController
    {
        #region Dependencies
        IZoneView _view;
        IZoneModel _model;

        public IZoneView View { get => _view; }
        protected IZoneModel Model { get => _model; }
        public void Initialize<TView, TModel>(TView view, TModel model)
            where TView : IZoneView
            where TModel : IZoneModel
        {
            _view = view;
            _model = model;

            _model.Player = view.Player;
        }
        #endregion

        #region Properties
        public Player Player => _model.Player;

        #endregion

        #region Methods
        public virtual void AddCard(ICardController card)
        {
            card.Zone = this;
            Model.AddCard(card);
        }

        public virtual ICardController RemoveCard(ICardController card)
        {
            _view.RemoveCardFromHolder(card.View);
            return _model.RemoveCard(card);
        }
        public virtual ICardController RemoveCard(int index)
        {
            return RemoveCard(_model.CardList[index]);
        }
        public Dictionary<StoneType, int> GetTotalCostOfZone()
        {
            return _model.GetTotalCostOfZone();
        }
        public virtual IEnumerator MoveCardToZone(ICardController card, int index = 0)
        {
            card.Zone = this;
            yield return _view.MoveCardToZone(card.View, index);
            _model.AddCard(card, index);
        }
        public virtual void AddCardToZone(ICardController card, int index = 0)
        {
            card.Zone = this;
            _view.AddCardToHolder(card.View, index);
            _model.AddCard(card, index);
        }
        
        #endregion

    }
}
