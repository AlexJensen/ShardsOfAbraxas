using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using Abraxas.Core;
using Abraxas.GameStates;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.Zones.Decks.Managers;
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
        IDeckManager _deckManager;

        // Zones
        IZoneManager _zoneManager;

        [Inject]
        public void Construct(Game.Settings settings,
                       IGameStateManager gameStateManager,
                       IDeckManager deckManager,
                       IZoneManager zoneManager,
                       IPlayerManager playerManager,
                       IManaManager manaManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _playerManager = playerManager;
            _manaManager = manaManager;
            _deckManager = deckManager;
        }
        #endregion

        #region Properties
        public bool GameStarted { get; private set; } = false;
        #endregion;

        #region Methods
        public void Start()
        {
            StartCoroutine(_gameStateManager.SwitchGameStateTo(States.GameNotStarted));
        }

        public IEnumerator StartGame()
        {
            if (!GameStarted)
            {
                GameStarted = true;
                 _deckManager.RequestBuildDecks();
                yield return new WaitForSeconds(.5f);
                _deckManager.RequestShuffleDeck(Player.Player1);
                _deckManager.RequestShuffleDeck(Player.Player2);
                yield return new WaitForSeconds(.5f);
                yield return Utilities.WaitForCoroutines(_zoneManager.MoveCardsFromDeckToHand(Player.Player1, _settings.Player1CardsToDrawAtStartOfGame),
                                                         _zoneManager.MoveCardsFromDeckToHand(Player.Player2, _settings.Player2CardsToDrawAtStartOfGame));
            }
        }
        public IEnumerator DrawStartOfTurnCardsForActivePlayer()
        {
            yield return _zoneManager.MoveCardsFromDeckToHand(_playerManager.ActivePlayer, _settings.CardsToDrawAtStartOfTurn);
        }
        public IEnumerator GenerateStartOfTurnManaForActivePlayer()
        {
            yield return _manaManager.GenerateManaFromDeckRatio(_playerManager.ActivePlayer, _manaManager.StartOfTurnMana);
            _manaManager.IncrementStartOfTurnManaAmount();
        }

        #endregion

        #region Server Methods
        public void RequestPurchaseCardAndMoveFromHandToCell(ICardController card, Point fieldPosition)
        {
            NetworkObject cardNetworkObject = card.View.NetworkObject;
            if (cardNetworkObject != null)
            {
                ulong cardNetworkObjectId = cardNetworkObject.NetworkObjectId;
                PurchaseCardAndMoveFromHandToCellServerRpc(cardNetworkObjectId, new Vector2Int(fieldPosition.X, fieldPosition.Y));
            }
        }
        [ServerRpc(RequireOwnership = false)]
        private void PurchaseCardAndMoveFromHandToCellServerRpc(ulong cardNetworkObjectId, Vector2Int fieldPosition)
        {
            NetworkObject cardNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[cardNetworkObjectId];
            if (cardNetworkObject != null)
            {
                if (cardNetworkObject.GetComponent<CardView>() is CardView card)
                {
                    PurchaseCardAndMoveFromHandToCell(card.Controller, fieldPosition);
                }
                if (!IsHost) PurchaseCardAndMoveFromHandToCellClientRpc(cardNetworkObjectId, fieldPosition);
            }
        }
        [ClientRpc]
        private void PurchaseCardAndMoveFromHandToCellClientRpc(ulong cardNetworkObjectId, Vector2Int fieldPosition)
        {
            NetworkObject cardNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[cardNetworkObjectId];
            if (cardNetworkObject != null)
            {
                if (cardNetworkObject.GetComponent<CardView>() is CardView card)
                {
                    PurchaseCardAndMoveFromHandToCell(card.Controller, fieldPosition);
                }
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