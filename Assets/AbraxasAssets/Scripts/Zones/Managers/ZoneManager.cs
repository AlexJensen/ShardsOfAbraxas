using Abraxas.Cards.Controllers;
using Abraxas.Players.Managers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Graveyards;
using Abraxas.Zones.Hands.Managers;
using System.Collections;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Managers
{
    class ZoneManager: IZoneManager
    {
        #region Dependencies
        // Zones
        readonly IHandManager _handManager;
        readonly IDeckManager _deckManager;
        readonly IGraveyardManager _graveyardManager;
        readonly IFieldManager _fieldManager;
        readonly IPlayerManager _playerManager;

        public ZoneManager(IHandManager handManager, IDeckManager deckManager, IGraveyardManager graveyardManager, IFieldManager fieldManager, IPlayerManager playerManager)
        {
            _handManager = handManager;
            _deckManager = deckManager;
            _graveyardManager = graveyardManager;
            _fieldManager = fieldManager;
            _playerManager = playerManager;
        }
        #endregion

        #region Methods
        public IEnumerator MoveCardsFromDeckToHand(Player player, int amount, int index = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                ICardController card = _deckManager.RemoveCard(player, index);
                if (!NetworkManager.Singleton.IsServer) card.Hidden = card.Owner != _playerManager.LocalPlayer;
                yield return _handManager.MoveCardToHand(player, card);
            }
        }
        public IEnumerator MoveCardsFromDeckToGraveyard(Player player, int amount, int index = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                ICardController card = _deckManager.RemoveCard(player, index);
                yield return _graveyardManager.MoveCardToGraveyard(player, card);
            }
        }
        public IEnumerator MoveCardFromHandToCell(ICardController card, Point fieldPosition)
        {
            _handManager.RemoveCard(card);
            card.Hidden = false;
            yield return _fieldManager.MoveCardToCell(card, fieldPosition);
            _fieldManager.AddCard(card, fieldPosition);
        }
        public IEnumerator MoveCardFromFieldToDeck(ICardController card)
        {
            _fieldManager.RemoveCard(card);
            yield return _deckManager.MoveCardToDeck(card.OriginalOwner, card);
        }
        public IEnumerator MoveCardFromFieldToGraveyard(ICardController card)
        {
            _fieldManager.RemoveCard(card);
            yield return _graveyardManager.MoveCardToGraveyard(card.OriginalOwner, card);
        }
        #endregion
    }
}
