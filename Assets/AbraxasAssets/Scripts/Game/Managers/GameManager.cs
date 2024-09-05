using Abraxas.Cards.Controllers;
using Abraxas.Cards.Managers;
using Abraxas.Core;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.GameStates;
using Abraxas.Manas;
using Abraxas.Network.Managers;
using Abraxas.Players.Managers;


using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;
using State = Abraxas.GameStates.GameStates;

namespace Abraxas.Games.Managers
{
    /// <summary>
    /// GameManager is responsible for handling complex game-logic level instructions and facilitating communication and delegation between multiple other manager classes to implement those instructions into game logic.
    /// </summary>
    public class GameManager : NetworkedManager, IGameManager
    {
        #region Settings
        Game.Settings _settings;
        #endregion

        #region Dependencies
        IGameStateManager _gameStateManager;
        IPlayerManager _playerManager;
        IManaManager _manaManager;
        ICardManager _cardManager;
        IEventManager _eventManager;

        // Zones
        IZoneManager _zoneManager;
        IDeckManager _deckManager;

        [Inject]
        public void Construct(Game.Settings settings,
                       IGameStateManager gameStateManager,
                       IZoneManager zoneManager,
                       IDeckManager deckManager,
                       IPlayerManager playerManager,
                       IManaManager manaManager,
                       ICardManager cardManager,
                       IEventManager eventManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _deckManager = deckManager;
            _playerManager = playerManager;
            _manaManager = manaManager;
            _cardManager = cardManager;
            _eventManager = eventManager;
        }
        #endregion

        #region Methods
        public void Start()
        {
            StartCoroutine(WaitForServerOrClient());
        }
        public IEnumerator WaitForServerOrClient()
        {
            while (!IsServer && !IsClient)
            {
                yield return null;
            }
            yield return _gameStateManager.InitializeState(State.GameNotStarted);
        }

        public IEnumerator StartGame()
        {
            yield return Utilities.WaitForCoroutines(DrawCard(Player.Player1, _settings.Player1CardsToDrawAtStartOfGame),
                                                     DrawCard(Player.Player2, _settings.Player2CardsToDrawAtStartOfGame));
        }

        public IEnumerator DrawCard(Player player, int amount = 1, int index = 0)
        {
            List<ICardController> cards = new();
            for (int i = 0; i < amount; i++)
            {
                var card = _deckManager.PeekCard(player, index);
                yield return _zoneManager.MoveCardFromDeckToHand(card, player);
                cards.Add(card);
            }
            yield return _eventManager.RaiseEvent(new Event_PlayerDrawsCards(player, cards));
        }

        public IEnumerator MillCard(Player player, int amount = 1, int index = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return _zoneManager.MoveCardFromDeckToGraveyard(_deckManager.PeekCard(player, index), player);
                yield return _eventManager.RaiseEvent(new Event_PlayerMillsCards(player, amount));
            }
        }

        public IEnumerator DrawStartOfTurnCardsForActivePlayer()
        {
            yield return DrawCard(_playerManager.ActivePlayer, _settings.CardsToDrawAtStartOfTurn);
        }
        public IEnumerator GenerateStartOfTurnManaForActivePlayer()
        {
            yield return _manaManager.GenerateManaFromDeckRatio(_playerManager.ActivePlayer, _manaManager.GetStartOfTurnMana(_playerManager.ActivePlayer));
            _manaManager.IncrementStartOfTurnManaAmount(_playerManager.ActivePlayer);
        }

        public bool IsAnyPlayerInputAvailable()
        {
            foreach (var card in _cardManager.Cards)
            {
                if (card.DeterminePlayability())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region Server Methods
        public void RequestPurchaseCardAndMoveFromHandToCell(ICardController card, Point fieldPosition)
        {
            if (!IsClient) return;
            int cardNetworkObjectId = _cardManager.GetCardIndex(card);
            PurchaseCardAndMoveFromHandToCellServerRpc(cardNetworkObjectId, new Vector2Int(fieldPosition.X, fieldPosition.Y));
        }
        [ServerRpc(RequireOwnership = false)]
        private void PurchaseCardAndMoveFromHandToCellServerRpc(int cardNetworkObjectId, Vector2Int fieldPosition)
        {

            ICardController card = _cardManager.GetCardFromIndex(cardNetworkObjectId);
            if (card != null)
            {
                StartCoroutine(PurchaseCardAndMoveFromHandToCell(card, fieldPosition));
                if (!IsHost) PurchaseCardAndMoveFromHandToCellClientRpc(cardNetworkObjectId, fieldPosition);
            }
        }
        [ClientRpc]
        private void PurchaseCardAndMoveFromHandToCellClientRpc(int cardNetworkObjectId, Vector2Int fieldPosition)
        {
            ICardController card = _cardManager.GetCardFromIndex(cardNetworkObjectId);
            if (card != null)
            {
                StartCoroutine(PurchaseCardAndMoveFromHandToCell(card, fieldPosition));
            }
        }
        private IEnumerator PurchaseCardAndMoveFromHandToCell(ICardController card, Vector2Int fieldPosition)
        {
            _manaManager.PurchaseCard(card);
            yield return _zoneManager.MoveCardFromHandToCell(card, new Point(fieldPosition.x, fieldPosition.y));
            if (!IsAnyPlayerInputAvailable() && _playerManager.LocalPlayer == _playerManager.ActivePlayer)
            {
                _gameStateManager.RequestNextGameState();
            }
        }
        #endregion
    }
}