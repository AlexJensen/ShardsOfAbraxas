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
        public Player Player => Model.Player;

        #endregion

        #region Methods
        public virtual ICardController RemoveCard(ICardController card)
        {
            UnityEngine.Debug.Log("ZoneController.RemoveCard");
            View.RemoveCardFromHolder(card.View);
            return Model.RemoveCard(card);
        }
        public virtual ICardController RemoveCard(int index)
        {
            UnityEngine.Debug.Log("ZoneController.RemoveCard (index)");
            return RemoveCard(Model.CardList[index]);
        }
        public Dictionary<StoneType, int> GetTotalCostOfZone()
        {
            return Model.GetTotalCostOfZone();
        }
        public virtual IEnumerator MoveCardToZone(ICardController card, int index = 0)
        {
            UnityEngine.Debug.Log("ZoneController.MoveCardToZone");
            card.Zone = this;
            yield return View.MoveCardToZone(card.View, index);
            Model.AddCard(card, index);
        }
        public virtual void AddCardToZone(ICardController card, int index = 0)
        {
            UnityEngine.Debug.Log("ZoneController.AddCardToZone");
            card.Zone = this;
            View.AddCardToHolder(card.View, index);
            Model.AddCard(card, index);
        }
        
        #endregion

    }
}
