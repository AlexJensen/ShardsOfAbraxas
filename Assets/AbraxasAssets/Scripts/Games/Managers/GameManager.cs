using Abraxas.Cards.Controllers;
using Abraxas.Cards.Managers;
using Abraxas.Core;
using Abraxas.GameStates;
using Abraxas.Manas;
using Abraxas.Network.Managers;
using Abraxas.Players.Managers;


using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Managers;
using System.Collections;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;
using State = Abraxas.GameStates.GameStates;
using Abraxas.Random.Managers;

namespace Abraxas.Games.Managers
{
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
                       ICardManager cardManager)
        {
            _settings = settings;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _deckManager = deckManager;
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
            yield return _gameStateManager.InitializeState(State.GameNotStarted);
        }

        public IEnumerator StartGame()
        {
            yield return Utilities.WaitForCoroutines(DrawCard(Player.Player1, _settings.Player1CardsToDrawAtStartOfGame),
                                                     DrawCard(Player.Player2, _settings.Player2CardsToDrawAtStartOfGame));
        }

        public IEnumerator DrawCard(Player player, int amount = 1, int index = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return _zoneManager.MoveCardFromDeckToHand(_deckManager.PeekCard(player, index), player);
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