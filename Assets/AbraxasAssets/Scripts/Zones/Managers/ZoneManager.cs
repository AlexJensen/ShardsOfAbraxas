using Abraxas.Cards.Controllers;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Graveyards;
using Abraxas.Zones.Hands.Managers;
using System;
using System.Collections;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Managers
{
	internal class ZoneManager : IZoneManager
	{
		#region Dependencies
		readonly IHandManager _handManager;
		readonly IDeckManager _deckManager;
		readonly IGraveyardManager _graveyardManager;
		readonly IFieldManager _fieldManager;
		readonly IEventManager _eventManager;

		public ZoneManager(IHandManager handManager, 
						   IDeckManager deckManager,
						   IGraveyardManager graveyardManager, 
						   IFieldManager fieldManager,
						   IEventManager eventManager) : base()
		{
			_handManager = handManager;
			_deckManager = deckManager;
			_graveyardManager = graveyardManager;
			_fieldManager = fieldManager;
			_eventManager = eventManager;
		}
		#endregion

		#region Methods
		public IEnumerator MoveCardFromDeckToHand(ICardController card, Player player)
		{
			yield return MoveCard(card, _deckManager.RemoveCard, _handManager.MoveCardToHand, player);
		}

		public IEnumerator MoveCardFromDeckToGraveyard(ICardController card, Player player)
		{
			yield return MoveCard(card, _deckManager.RemoveCard, _graveyardManager.MoveCardToGraveyard, player);
		}

		public IEnumerator MoveCardFromHandToCell(ICardController card, Point fieldPosition)
		{
			_handManager.RemoveCard(card);
			yield return _fieldManager.MoveCardToCell(card, fieldPosition);
			yield return _eventManager.RaiseEvent(typeof(CardChangedZonesEvent), new CardChangedZonesEvent(card));
		}

		public IEnumerator MoveCardFromFieldToDeck(ICardController card, Player player)
		{
			_fieldManager.RemoveCard(card);
			yield return _deckManager.MoveCardToDeck(card.Owner, card);
			yield return _eventManager.RaiseEvent(typeof(CardChangedZonesEvent), new CardChangedZonesEvent(card));
		}

		public IEnumerator MoveCardFromFieldToGraveyard(ICardController card, Player player)
		{
			_fieldManager.RemoveCard(card);
			yield return _graveyardManager.MoveCardToGraveyard(player, card);
			yield return _eventManager.RaiseEvent(typeof(CardChangedZonesEvent), new CardChangedZonesEvent(card));
		}

		private IEnumerator MoveCard(ICardController card, Action<Player, ICardController> removeMethod,
									 Func<Player, ICardController, IEnumerator> addMethod, Player player)
		{
			removeMethod(player, card);
			yield return addMethod(player, card);
			yield return _eventManager.RaiseEvent(typeof(CardChangedZonesEvent), new CardChangedZonesEvent(card));
		}
		#endregion
	}
}