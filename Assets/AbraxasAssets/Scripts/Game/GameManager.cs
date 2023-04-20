using System;
using System.Collections;
using UnityEngine;
using Zenject;
using Abraxas.Core;
using Abraxas.Zones.Hands;
using Abraxas.Manas;
using Abraxas.GameStates;
using Abraxas.Players;
using Abraxas.Zones.Fields;
using Abraxas.Zones.Graveyards;
using Abraxas.Zones.Decks;
using Abraxas.UI;
using Abraxas.CardViewers;
using Abraxas.Cards;

using States = Abraxas.GameStates.GameStates;
using Player = Abraxas.Players.Players;
using Abraxas.Events;

namespace Abraxas.Game
{
    public class GameManager : MonoBehaviour, IGameManager
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
        //IMenuManager _menuManager;

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
                       //IMenuManager menuManager,
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
            //_menuManager = menuManager;
            _handManager = handManager;
            _deckManager = deckManager;
            _graveyardManager = graveyardManager;
            _fieldManager = fieldManager;
        }
        #endregion

        #region Properties
        public Player ActivePlayer => _playerManager.ActivePlayer;
        #endregion

        #region Methods
        public void Start()
        {
            StartCoroutine(_gameStateManager.SwitchToState(States.GameNotStarted));
        }

        public IEnumerator StartGame()
        {
            yield return Utilities.WaitForCoroutines(this,
                            _deckManager.ShuffleDeck(Player.Player1),
                            _deckManager.ShuffleDeck(Player.Player2));
            yield return Utilities.WaitForCoroutines(this,
                            MoveCardsFromDeckToHand(Player.Player1, _settings.Player1CardsToDrawAtStartOfGame),
                            MoveCardsFromDeckToHand(Player.Player2, _settings.Player2CardsToDrawAtStartOfGame));
        }

        public IEnumerator BeginNextGameState()
        {
            yield return _gameStateManager.BeginNextGameState();
        }

        public IEnumerator DrawStartOfTurnCardsForActivePlayer()
        {
            yield return MoveCardsFromDeckToHand(_playerManager.ActivePlayer, _settings.CardsToDrawAtStartOfTurn);
        }

        public IEnumerator GenerateStartOfTurnManaForActivePlayer()
        {
            yield return _manaManager.GenerateManaFromDeckRatio(_playerManager.ActivePlayer, _manaManager.StartOfTurnManaAmount);
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

        public void ToggleActivePlayer()
        {
            _playerManager.ToggleActivePlayer();
        }

        public void ModifyPlayerHealth(Player player, int amount)
        {
            _playerManager.ModifyPlayerHealth(player, amount);
        }

        public IEnumerator ShowCardDetail(Card card)
        {
            yield return _cardViewerManager.ShowCardDetail(card); 
        }

        public IEnumerator HideCardDetail()
        {
           yield return _cardViewerManager.HideCardDetail();
        }

        public IEnumerator MoveCardAndFight(Card card, Vector2Int vector2Int)
        {
            throw new NotImplementedException();
        }

        public void PurchaseCard(Card card)
        {
            _manaManager.CanPurchaseCard(card);
        }

        public bool CanPurchaseCard(Card card)
        {
            return _manaManager.CanPurchaseCard(card);
        }

        public IEnumerator MoveCardToHand(Card card)
        {
            yield return _handManager.MoveCardToHand(card.Owner, card);
        }

        public object MoveCardFromCellToCell(Cell source, Cell destination)
        {
            throw new NotImplementedException();
        }




        //Everything below here is old and will be refactored or removed


        //public bool CanPurchaseCard(Card card)
        //{
        //    Mana currentPlayerMana = GetPlayerMana(card.Owner);
        //    foreach (var cost in card.TotalCosts)
        //    {
        //        ManaType result = currentPlayerMana.ManaTypes.Find(x => x.Type == cost.Key);
        //        if (!result || result.Amount < cost.Value)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public void PurchaseCard(Card card)
        //{
        //    Mana currentPlayerMana = GetPlayerMana(card.Owner);
        //    foreach (var cost in card.TotalCosts)
        //    {
        //        ManaType result = currentPlayerMana.ManaTypes.Find(x => x.Type == cost.Key);
        //        result.Amount -= cost.Value;
        //    }
        #endregion
    }
}