using Abraxas.Cards.Controllers;
using Abraxas.Cards.Managers;
using Abraxas.Cards.Views;
using Abraxas.Core;
using Abraxas.GameStates;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.Zones.Managers;
using System.Collections;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;
using States = Abraxas.GameStates.GameStates;

namespace Abraxas.Game.Managers
{
    public class GameManager : NetworkBehaviour, IGameManager
    {
        #region Settings
        Game.Settings _settings;
        #endregion

        #region Dependencies
        IGameStateManager _gameStateManager;
        IPlayerManager _playerManager;
        IManaManager _manaManager;
        ICardManager _cardManager;

        // Zones
        IZoneManager _zoneManager;

        [Inject]
        public void Construct(Game.Settings settings,
                       IGameStateManager gameStateManager,
                       IZoneManager zoneManager,
                       IPlayerManager playerManager,
                       IManaManager manaManager,
                       ICardManager cardManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _playerManager = playerManager;
            _manaManager = manaManager;
            _cardManager = cardManager;
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
            yield return _gameStateManager.InitializeState(States.GameNotStarted);
        }
        public IEnumerator StartGame()
        {
            Debug.Log($"StartGame");
            yield return Utilities.WaitForCoroutines(_zoneManager.MoveCardsFromDeckToHand(Player.Player1, _settings.Player1CardsToDrawAtStartOfGame),
                                                     _zoneManager.MoveCardsFromDeckToHand(Player.Player2, _settings.Player2CardsToDrawAtStartOfGame));
            Debug.Log($"StartGameCompleted");
        }
        public IEnumerator DrawStartOfTurnCardsForActivePlayer()
        {
            Debug.Log($"DrawStartOfTurnCardsForActivePlayer: {_playerManager.ActivePlayer}");
            yield return _zoneManager.MoveCardsFromDeckToHand(_playerManager.ActivePlayer, _settings.CardsToDrawAtStartOfTurn);
        }
        public IEnumerator GenerateStartOfTurnManaForActivePlayer()
        {
            Debug.Log($"GenerateStartOfTurnManaForActivePlayer: {_playerManager.ActivePlayer}");
            yield return _manaManager.GenerateManaFromDeckRatio(_playerManager.ActivePlayer, _manaManager.StartOfTurnMana);
            _manaManager.IncrementStartOfTurnManaAmount();
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
                PurchaseCardAndMoveFromHandToCell(card, fieldPosition);
                if (!IsHost) PurchaseCardAndMoveFromHandToCellClientRpc(cardNetworkObjectId, fieldPosition);
            }
        }
        [ClientRpc]
        private void PurchaseCardAndMoveFromHandToCellClientRpc(int cardNetworkObjectId, Vector2Int fieldPosition)
        {
            ICardController card = _cardManager.GetCardFromIndex(cardNetworkObjectId);
            if (card != null)
            {
                PurchaseCardAndMoveFromHandToCell(card, fieldPosition);
            }
        }
        private void PurchaseCardAndMoveFromHandToCell(ICardController card, Vector2Int fieldPosition)
        {
            _manaManager.PurchaseCard(card);
            StartCoroutine(_zoneManager.MoveCardFromHandToCell(card, new Point(fieldPosition.x, fieldPosition.y)));
        }
        #endregion
    }
}