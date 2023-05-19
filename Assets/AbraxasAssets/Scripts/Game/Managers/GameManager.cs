using Abraxas.Cards.Controllers;
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

        // Zones
        IZoneManager _zoneManager;

        [Inject]
        public void Construct(Game.Settings settings,
                       IGameStateManager gameStateManager,
                       IZoneManager zoneManager,
                       IPlayerManager playerManager,
                       IManaManager manaManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _playerManager = playerManager;
            _manaManager = manaManager;
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
                _zoneManager.BuildDecks();
                yield return Utilities.WaitForCoroutines(
                                _zoneManager.ShuffleDeck(Player.Player1),
                                _zoneManager.ShuffleDeck(Player.Player2));
                yield return new WaitForSeconds(.1f);
                Debug.Log($"GameManagerDrawingStartOfGameCards");
                yield return Utilities.WaitForCoroutines(
                                _zoneManager.MoveCardsFromDeckToHand(Player.Player1, _settings.Player1CardsToDrawAtStartOfGame),
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
        public void RequestPurchaseCardAndMoveFromHandToCell(ICardController card, Point fieldPosition)
        {
            PurchaseCardAndMoveFromHandToCellServerRpc(card.View.NetworkBehaviourReference, new Vector2Int(fieldPosition.X, fieldPosition.Y));
        }
        private void PurchaseCardAndMoveFromHandToCell(ICardView card, Vector2Int fieldPosition)
        {
            _manaManager.PurchaseCard(card.Controller);
            StartCoroutine(_zoneManager.MoveCardFromHandToCell(card.Controller, new Point(fieldPosition.x, fieldPosition.y)));
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