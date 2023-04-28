using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Abraxas.Core;
using Abraxas.Zones.Hands;
using Abraxas.Manas;
using Abraxas.GameStates;
using Abraxas.Players;
using Abraxas.Stones;
using Abraxas.UI;
using Abraxas.CardViewers;
using Abraxas.Cards;
using Abraxas.Events;
using Abraxas.Zones.Fields;
using Abraxas.Zones.Graveyards;
using Abraxas.Zones.Decks;

using States = Abraxas.GameStates.GameStates;
using Player = Abraxas.Players.Players;
using Unity.Netcode;

namespace Abraxas.Game
{
    public class GameManager : NetworkBehaviour, IGameManager
    {
        #region Settings
        Settings _settings;
        [Serializable]
        public class Settings
        {
            public int Player1CardsToDrawAtStartOfGame;
            public int Player2CardsToDrawAtStartOfGame;
            public int CardsToDrawAtStartOfTurn;
        }
        #endregion

        #region Dependencies
        IGameStateManager _gameStateManager;
        IEventManager _eventManager;
        IPlayerManager _playerManager;
        ICardViewerManager _cardViewerManager;
        IManaManager _manaManager;
        IMenuManager _menuManager;

        // Zones
        IHandManager _handManager;
        IDeckManager _deckManager;
        IGraveyardManager _graveyardManager;
        IFieldManager _fieldManager;

        [Inject]
        void Construct(Settings settings,
                       IGameStateManager gameStateManager, 
                       IEventManager eventManager,
                       IPlayerManager playerManager,
                       ICardViewerManager cardViewerManager,
                       IManaManager manaManager,
                       IMenuManager menuManager,
                       IHandManager handManager,
                       IDeckManager deckManager,
                       IGraveyardManager graveyardManager,
                       IFieldManager fieldManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            _eventManager = eventManager;
            _playerManager = playerManager;
            _cardViewerManager = cardViewerManager;
            _manaManager = manaManager;
            _menuManager = menuManager;
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
            yield return Utilities.WaitForCoroutines(this,
                            _deckManager.ShuffleDeck(Player.Player1),
                            _deckManager.ShuffleDeck(Player.Player2));
            yield return new WaitForSeconds(.1f);
            yield return Utilities.WaitForCoroutines(this,
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
                Card card = _deckManager.RemoveCard(player, index);
                yield return _handManager.MoveCardToHand(player, card);
            }
        }
        public IEnumerator MoveCardsFromDeckToGraveyard(Player player, int amount, int index = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                Card card = _deckManager.RemoveCard(player, index);
                yield return _graveyardManager.MoveCardToGraveyard(player, card);
            }
        }
        public IEnumerator MoveCardFromHandToCell(Card card, Vector2Int fieldPosition)
        {
            _handManager.RemoveCard(card.Owner, card);
            yield return _fieldManager.MoveCardToCell(card, fieldPosition);
            _fieldManager.AddCard(card, fieldPosition);
        }
        public IEnumerator MoveCardFromFieldToDeck(Card card)
        {
            _fieldManager.RemoveCard(card);
            yield return _deckManager.MoveCardToDeck(card.Owner, card);
            _deckManager.ShuffleDeck(card.Owner);
        }
        public IEnumerator MoveCardFromFieldToGraveyard(Card card)
        {
            _fieldManager.RemoveCard(card);
            yield return _graveyardManager.MoveCardToGraveyard(card.Owner, card);
        }

        public void PurchaseCardAndMoveFromHandToCell(Card card, Vector2Int fieldPosition)
        {
            PurchaseCardAndMoveFromHandToCellServerRpc(card, fieldPosition);
        }
        #endregion

        #region Server Methods
        [ServerRpc(RequireOwnership = false)]
        private void PurchaseCardAndMoveFromHandToCellServerRpc(NetworkBehaviourReference cardReference, Vector2Int fieldPosition)
        {
            if (!IsServer) return;
            PurchaseCardAndMoveFromHandToCellClientRpc(cardReference, fieldPosition);
        }

        [ClientRpc]
        private void PurchaseCardAndMoveFromHandToCellClientRpc(NetworkBehaviourReference cardReference, Vector2Int fieldPosition)
        {
            if (!IsClient) return;

            if (cardReference.TryGet(out Card card))
            {
                _handManager.RemoveCard(card.Owner, card);
                _manaManager.PurchaseCard(card);
               StartCoroutine(_fieldManager.MoveCardToCell(card, fieldPosition));
            }
        }
        #endregion
    }
}