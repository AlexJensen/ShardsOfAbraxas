using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Manas;
using Abraxas.StatBlocks;
using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using Abraxas.Zones.Decks.Controllers;
using Abraxas.Zones.Decks.Models;
using Abraxas.Zones.Decks.Views;
using Abraxas.Zones.Factories;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Decks.Managers
{
    public class DeckManager : NetworkBehaviour, IDeckManager
    {
        #region Dependencies
        [SerializeField]
        List<DeckView> _deckViews;
        readonly List<IDeckController> _decks = new();
        CardController.Factory _cardFactory;
        IManaManager _manaManager;
        ZoneFactory<IDeckView, DeckController, DeckModel> _deckFactory;
        [Inject]
        void Construct(CardController.Factory cardFactory, ZoneFactory<IDeckView, DeckController, DeckModel> deckFactory, IManaManager manaManager)
        {
            _cardFactory = cardFactory;
            _deckFactory = deckFactory;
            _manaManager = manaManager;
        }
        #endregion

        #region Methods

        public Dictionary<StoneType, int> GetDeckCost(Player player)
        {
            return GetPlayerDeck(player).GetTotalCostOfZone();
        }
        public IEnumerator MoveCardToDeck(Player player, ICardController card)
        {
            yield return GetPlayerDeck(player).MoveCardToZone(card);
        }
        public ICardController RemoveCard(Player player, int index)
        {
            return GetPlayerDeck(player).RemoveCard(index);
        }
        
        private IDeckController GetPlayerDeck(Player player)
        {
            return _decks.Find(x => x.Player == player);
        }

        private List<CardData> GenerateCardDataList(Player player)
        {
            List<CardData> cardDataList = new();

            string[] titles =
                {
                "I", "II", "III","IV","V","VI","VII","VIII", "IX", "X"
            };

            for (int i = 0; i < 10; i++)
            {
                int cost = Random.Range(1, 20);
                StoneType type = (StoneType)Random.Range(0, 11);
                int atk = Random.Range(1, 5);
                int def = Random.Range(1, 5);
                int mv = Random.Range(1, 5);

                for (int j = 0; j < 4; j++)
                {
                    CardData cardData = new CardData
                    {
                        Title = titles[i],
                        Owner = player,
                        OriginalOwner = player,
                        Stones = new(),
                        StatBlock = new StatBlockData
                        {
                            Cost = cost,
                            StoneType = type,
                            [StatValues.ATK] = atk,
                            [StatValues.DEF] = def,
                            [StatValues.MV] = mv
                        }
                    };

                    cardDataList.Add(cardData);
                }
            }

            return cardDataList;
        }
        #endregion

        #region Server Methods
        public void RequestShuffleDeck(Player player)
        {
            if (!IsServer) return;

            int randomSeed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(randomSeed);
            ShuffleDeckClientRpc(player, randomSeed);
            if (IsHost) return;
            GetPlayerDeck(player).Shuffle();
        } 
        [ClientRpc]
        private void ShuffleDeckClientRpc(Player player, int seed)
        {
            Random.InitState(seed);
            GetPlayerDeck(player).Shuffle();
        }
        public void RequestBuildDecks()
        {
            if (!IsServer) return;

            List<CardData> cardDataListPlayer1 = GenerateCardDataList(Player.Player1);
            List<CardData> cardDataListPlayer2 = GenerateCardDataList(Player.Player2);

            CardDataListWrapper wrapperPlayer1 = new CardDataListWrapper
            {
                CardDataList = cardDataListPlayer1
            };
            CardDataListWrapper wrapperPlayer2 = new CardDataListWrapper
            {
                CardDataList = cardDataListPlayer2
            };

            string serializedCardDataPlayer1 = JsonUtility.ToJson(wrapperPlayer1);
            string serializedCardDataPlayer2 = JsonUtility.ToJson(wrapperPlayer2);

            BuildDecksClientRpc(Player.Player1, serializedCardDataPlayer1);
            BuildDecksClientRpc(Player.Player2, serializedCardDataPlayer2);

            if (IsHost) return;
            BuildDeck(cardDataListPlayer1, Player.Player1);
            BuildDeck(cardDataListPlayer2, Player.Player2);
        }
        [ClientRpc]
        public void BuildDecksClientRpc(Player player, string serializedCardData)
        {
            CardDataListWrapper wrapper = JsonUtility.FromJson<CardDataListWrapper>(serializedCardData);
            List<CardData> cardDataList = wrapper.CardDataList;
            BuildDeck(cardDataList, player);
        }
        private void BuildDeck(List<CardData> cardDataList, Player player)
        {
            foreach (var deckView in _deckViews)
            {
                var deckController = _deckFactory.Create(deckView);
                if (player != deckController.Player) continue;
                _decks.Add(deckController);
                foreach (var cardData in cardDataList)
                {
                    CardData modifiedCardData = cardData;
                    modifiedCardData.Owner = deckController.Player;
                    modifiedCardData.OriginalOwner = deckController.Player;
                    deckController.AddCardToZone(_cardFactory.Create(modifiedCardData, deckController.Player));
                }
                _manaManager.InitializeManaFromDeck(deckController);
            }
        }
        #endregion
    }
}
