using Abraxas.Cards.Controllers;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Stones;
using Abraxas.Zones.Models;
using Abraxas.Zones.Views;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Controllers
{
    abstract class ZoneController : IZoneController,
        IGameEventListener<Event_LocalPlayerChanged>
    {
        #region Dependencies
        IZoneView _view;
        IZoneModel _model;

        IEventManager _eventManager;
        [Inject]
        public void Construct(IEventManager eventManager)
        {
            _eventManager = eventManager;

            _eventManager.AddListener(this);
        }

        public void Initialize<TView, TModel>(TView view, TModel model)
            where TView : IZoneView
            where TModel : IZoneModel
        {
            _view = view;
            _model = model;

            _model.Player = view.Player;
        }

        public void OnDestroy()
        {
            _eventManager.RemoveListener(this);
        }
        #endregion

        #region Properties
        public Player Player => _model.Player;

        public int TotalCardsInZone => _model.CardList.Count;
        protected IZoneView View => _view;
        protected IZoneModel Model => _model;
        #endregion

        #region Methods
        public virtual void RemoveCard(ICardController card)
        {
            _view.RemoveCardFromHolder(card);
            _model.RemoveCard(card);
        }

        public virtual ICardController PeekCard(int index)
        {
            if (index >= _model.CardList.Count || index < 0)
                return null;
            return _model.CardList[index];
        }


        public Dictionary<StoneType, int> GetTotalCostOfZone()
        {
            return _model.GetTotalCostOfZone();
        }
        public virtual IEnumerator MoveCardToZone(ICardController card, int index = 0)
        {
            yield return _view.MoveCardToZone(card, index);
            AddCard(card, index);
        }
        public virtual void AddCard(ICardController card, int index = 0)
        {
            card.Zone = this;
            _view.AddCardToHolder(card, index);
            _model.AddCard(card, index);

        }

        public virtual IEnumerator OnEventRaised(Event_LocalPlayerChanged eventData)
        {
            _model.Player = _view.Player;
            yield break;
        }

        public bool ShouldReceiveEvent(Event_LocalPlayerChanged eventData)
        {
            return true;
        }
        #endregion

    }
}
