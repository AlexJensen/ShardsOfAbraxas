using System;
using System.Collections;
using UnityEngine;
using Zenject;
using Abraxas.Core;
using Abraxas.Zones.Hands;
using Abraxas.Manas;
using Abraxas.GameStates;
using Abraxas.Players;
using Abraxas.UI;
using Abraxas.CardViewers;
using Abraxas.Events;
using Abraxas.Zones.Fields;
using Abraxas.Zones.Graveyards;
using Abraxas.Zones.Decks;

using States = Abraxas.GameStates.GameStates;
using Player = Abraxas.Players.Players;
using Unity.Netcode;
using Abraxas.Cards.Controllers;
using System.Drawing;
using Abraxas.Cards.Views;

namespace Abraxas.Game
{
    public class GameManager : NetworkBehaviour, IGameManager
    {
        #region Settings
        Game.Settings _settings;
        #endregion

        #region Dependencies
        IGameStateManager _gameStateManager;
        //IEventManager _eventManager;
        IPlayerManager _playerManager;
        //ICardViewerManager _cardViewerManager;
        IManaManager _manaManager;
        //IMenuManager _menuManager;

        // Zones
        IHandManager _handManager;
        IDeckManager _deckManager;
        IGraveyardManager _graveyardManager;
        IFieldManager _fieldManager;

        [Inject]
        public void Construct(Game.Settings settings,
                       IGameStateManager gameStateManager, 
                       //IEventManager eventManager,
                       IPlayerManager playerManager,
                       //ICardViewerManager cardViewerManager,
                       IManaManager manaManager,
                       //IMenuManager menuManager,
                       IHandManager handManager,
                       IDeckManager deckManager,
                       IGraveyardManager graveyardManager,
                       IFieldManager fieldManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            //_eventManager = eventManager;
            _playerManager = playerManager;
            //_cardViewerManager = cardViewerManager;
            _manaManager = manaManager;
            //_menuManager = menuManager;
            _handManager = handManager;
            _deckManager = deckManager;
            _graveyardManager = graveyardManager;
            _fieldManager = fieldManager;
        }
        #endregion

        #region Methods
        public void Start()
        {
            StartCoroutine(_gameStateManager.SwitchGameStateTo(States.GameNotStarted));
        }

        public IEnumerator StartGame()
        {
            yield return Utilities.WaitForCoroutines(
                            _deckManager.ShuffleDeck(Player.Player1),
                            _deckManager.ShuffleDeck(Player.Player2));
            yield return new WaitForSeconds(.1f);
            yield return Utilities.WaitForCoroutines(
                            MoveCardsFromDeckToHand(Player.Player1, _settings.Player1CardsToDrawAtStartOfGame),
                            MoveCardsFromDeckToHand(Player.Player2, _settings.Player2CardsToDrawAtStartOfGame));
        }
        public IEnumerator DrawStartOfTurnCardsForActivePlayer()
        {
            yield return new WaitForSeconds(.1f);
            yield return MoveCardsFromDeckToHand(_playerManager.ActivePlayer, _settings.CardsToDrawAtStartOfTurn);
        }
        public IEnumerator GenerateStartOfTurnManaForActivePlayer()
        {
            yield return _manaManager.GenerateManaFromDeckRatio(_playerManager.ActivePlayer, _manaManager.StartOfTurnMana);
            _manaManager.IncrementStartOfTurnManaAmount();
        }
        public IEnumerator MoveCardsFromDeckToHand(Player player, int amount, int index = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                ICardController card = _deckManager.RemoveCard(player, index);
                card.Hidden = card.Owner != _playerManager.LocalPlayer;
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
            _handManager.RemoveCard(card.OriginalOwner, card);
            card.Hidden = false;
            yield return _fieldManager.MoveCardToCell(card, fieldPosition);
            _fieldManager.AddCard(card, fieldPosition);
        }
        public IEnumerator MoveCardFromFieldToDeck(ICardController card)
        {
            _fieldManager.RemoveCard(card);
            yield return _deckManager.MoveCardToDeck(card.OriginalOwner, card);
            _deckManager.ShuffleDeck(card.OriginalOwner);
        }
        public IEnumerator MoveCardFromFieldToGraveyard(ICardController card)
        {
            _fieldManager.RemoveCard(card);
            yield return _graveyardManager.MoveCardToGraveyard(card.OriginalOwner, card);
        }

        public void RequestPurchaseCardAndMoveFromHandToCell(ICardController card, Point fieldPosition)
        {
            PurchaseCardAndMoveFromHandToCellServerRpc(card.View.NetworkBehaviourReference, new Vector2Int(fieldPosition.X, fieldPosition.Y));
        }

        private void PurchaseCardAndMoveFromHandToCell(ICardView card, Vector2Int fieldPosition)
        {
            _manaManager.PurchaseCard(card.Controller);
            StartCoroutine(MoveCardFromHandToCell(card.Controller, new Point(fieldPosition.x, fieldPosition.y)));
        }
        #endregion

        #region Server Methods
        [ServerRpc(RequireOwnership = false)]
        private void PurchaseCardAndMoveFromHandToCellServerRpc(NetworkBehaviourReference cardReference, Vector2Int fieldPosition)
        {
            if (cardReference.TryGet(out CardView card))
            {
                PurchaseCardAndMoveFromHandToCell(card, fieldPosition);
            }
            PurchaseCardAndMoveFromHandToCellClientRpc(cardReference, fieldPosition);
        }

        [ClientRpc]
        private void PurchaseCardAndMoveFromHandToCellClientRpc(NetworkBehaviourReference cardReference, Vector2Int fieldPosition)
        {
            if (cardReference.TryGet(out CardView card))
            {
                PurchaseCardAndMoveFromHandToCell(card, fieldPosition);
            }
        }
        #endregion
    }
}