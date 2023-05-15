using System.Collections;
using UnityEngine;
using Zenject;
using Abraxas.Core;
using Abraxas.Manas;
using Abraxas.GameStates;

using States = Abraxas.GameStates.GameStates;
using Player = Abraxas.Players.Players;
using Unity.Netcode;
using Abraxas.Cards.Controllers;
using System.Drawing;
using Abraxas.Cards.Views;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Players.Managers;
using Abraxas.Zones.Managers;

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
        IDeckManager _deckManager;


        [Inject]
        public void Construct(Game.Settings settings,
                       IGameStateManager gameStateManager,
                       IZoneManager zoneManager,
                       IPlayerManager playerManager,
                       IManaManager manaManager,
                       IDeckManager deckManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _playerManager = playerManager;
            _manaManager = manaManager;
            _deckManager = deckManager;
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
                            _zoneManager.MoveCardsFromDeckToHand(Player.Player1, _settings.Player1CardsToDrawAtStartOfGame),
                            _zoneManager.MoveCardsFromDeckToHand(Player.Player2, _settings.Player2CardsToDrawAtStartOfGame));
        }
        public IEnumerator DrawStartOfTurnCardsForActivePlayer()
        {
            yield return new WaitForSeconds(.1f);
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