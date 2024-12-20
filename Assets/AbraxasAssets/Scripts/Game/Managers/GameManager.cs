using Abraxas.AI.Managers;
using Abraxas.Cards.Controllers;
using Abraxas.Cards.Managers;
using Abraxas.Cells.Controllers;
using Abraxas.Core;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.GameStates;
using Abraxas.Health.Managers;
using Abraxas.Manas;
using Abraxas.Menus;
using Abraxas.Network.Managers;
using Abraxas.Players.Managers;
using Abraxas.UI;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Fields.Managers;
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
    /// GameManager is an executive manager class that orchestrates functionalities of multiple other managers to produce game logic. 
    /// </summary>
    public class GameManager : NetworkedManager, IGameManager
    {
        #region Settings
        Game.Settings _settings;
        #endregion

        #region Dependencies
        IGameStateManager _gameStateManager;
        IPlayerManager _playerManager;
        IPlayerHealthManager _playerHealthManager;
        IManaManager _manaManager;
        ICardManager _cardManager;
        IEventManager _eventManager;
        IUIManager _uiManager;
        IAIManager _aiManager;

        // Zones
        IZoneManager _zoneManager;
        IDeckManager _deckManager;
        IFieldManager _fieldManager;

        [Inject]
        public void Construct(Game.Settings settings,
                       IGameStateManager gameStateManager,
                       IZoneManager zoneManager,
                       IDeckManager deckManager,
                       IFieldManager fieldManager,
                       IPlayerManager playerManager,
                       IPlayerHealthManager playerHealthManager,
                       IManaManager manaManager,
                       ICardManager cardManager,
                       IEventManager eventManager,
                       IUIManager uiWindowManager,
                       IAIManager aIManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _deckManager = deckManager;
            _fieldManager = fieldManager;
            _playerManager = playerManager;
            _playerHealthManager = playerHealthManager;
            _manaManager = manaManager;
            _cardManager = cardManager;
            _eventManager = eventManager;
            _uiManager = uiWindowManager;
            _aiManager = aIManager;
        }
        #endregion

        #region Fields
        int overheat = 1;

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
                if (card == null)
                {
                    yield return _playerHealthManager.ModifyPlayerHealth(player, -overheat);
                    overheat *= 2;
                }
                else
                { 
                    yield return _zoneManager.MoveCardFromDeckToHand(card, player);
                    cards.Add(card);
                }
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
        public void GenerateStartOfTurnManaForActivePlayer()
        {
            if (!IsServer) return;
            _manaManager.GenerateManaFromDeckRatio(_playerManager.ActivePlayer, _manaManager.GetStartOfTurnMana(_playerManager.ActivePlayer));
            _manaManager.IncrementStartOfTurnManaAmount(_playerManager.ActivePlayer);
        }

        public bool IsAnyPlayerInputAvailable()
        {
            if (_fieldManager.GetOpenCells(_playerManager.ActivePlayer).Length == 0)
            {
                return false;
            }
            foreach (var card in _cardManager.Cards)
            {
                if (card.DeterminePlayability())
                {
                    return true;
                }
            }
            return false;
        }

        public List<ICardController> GetAllPlayableCards()
        {

           List<ICardController> cards = new();
            foreach (var card in _cardManager.Cards)
            {
                if (card.DeterminePlayability())
                {
                    cards.Add(card);
                }
            }
            return cards;
        }

        public ICellController[] GetAvailableCells()
        {
            return _fieldManager.GetOpenCells(_playerManager.ActivePlayer);
        }

        public IEnumerator EndGame(Player player)
        {
            var winningPlayer = player == Player.Player1 ? Player.Player2 : Player.Player1;
            _uiManager.SetFade(true);
            var gameOverPopup = _uiManager.PushMenu<GameOverPopup>() as GameOverPopup;
            gameOverPopup.SetWinner(_playerManager.LocalPlayer == Player.Player1 ? winningPlayer : player);
            while (true) {
                yield return null;
            }
        }
        #endregion


        #region Server Methods
        public void PurchaseCardAndMoveFromHandToCell(ICardController card, Point fieldPosition)
        {
            if (!IsServer) return;
            StartCoroutine(PurchaseCardAndMoveFromHandToCell(card, new Vector2Int(fieldPosition.X, fieldPosition.Y)));

            if (!IsHost)
            {
                int cardNetworkObjectId = _cardManager.GetCardIndex(card);
                PurchaseCardAndMoveFromHandToCellClientRpc(cardNetworkObjectId, new Vector2Int(fieldPosition.X, fieldPosition.Y));
            }
        }

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
            if (!IsAnyPlayerInputAvailable() && !(_playerManager.ActivePlayer == Player.Player1? _aiManager.IsPlayer1AI: _aiManager.IsPlayer2AI))
            {
                _gameStateManager.RequestNextGameState();
            }
        }
        #endregion
    }
}