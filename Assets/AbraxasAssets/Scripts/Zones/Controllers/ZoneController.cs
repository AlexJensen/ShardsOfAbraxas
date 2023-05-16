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

        protected IZoneView View { get => _view; }
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
        public Player Player => Model.Player;
        #endregion

        #region Methods
        public virtual ICardController RemoveCard(ICardController card)
        {
            return Model.RemoveCard(card);  
        }

        public Dictionary<StoneType, int> GetTotalCostOfZone()
        {
            return Model.GetTotalCostOfZone();
        }

        public virtual IEnumerator AddCard(ICardController card, int index = 0)
        {
            yield return View.MoveCardToZone(card.View, index);
            Model.AddCard(card, index);
        }
        public virtual ICardController RemoveCard(int index)
        {
            return Model.RemoveCard(index);
        }
        #endregion

    }
}
