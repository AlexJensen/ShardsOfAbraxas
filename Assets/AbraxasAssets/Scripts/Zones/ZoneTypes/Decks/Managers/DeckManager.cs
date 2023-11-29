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
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;
using Random = UnityEngine.Random;

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

        #region Fields
        private int clientAcknowledgments = 0;
        private bool isWaitingForClientAcknowledgments = false;
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
                
                StoneType type = (StoneType)Random.Range(0, 11);
                int atk = GetWeightedRandom(1, 15, 3);
                int def = GetWeightedRandom(1, 15, 3);
                int mv = GetWeightedRandom(1, 5, 2);
                int cost = (atk * 2) + (def * 2) + (mv * 4);
                int imageIndex = Random.Range(0, 4);

                for (int j = 0; j < 4; j++)
                {
                    CardData cardData = new()
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
                        },
                        ImageIndex = imageIndex
                    };
                    cardDataList.Add(cardData);
                }
            }
            return cardDataList;
        }

        private int GetWeightedRandom(int min, int max, int biasFactor)
        {
            int result = Random.Range(min, max);
            for (int i = 1; i < biasFactor; i++)
            {
                result = Math.Min(result, Random.Range(min, max));
            }
            return result;
        }
        #endregion

        #region Server Methods
        #region Server Side
        public IEnumerator ShuffleDeck(Player player)
        {
            if (!IsServer) yield break;
            int randomSeed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(randomSeed);
            if (!IsHost) GetPlayerDeck(player).Shuffle();
            ShuffleDeckClientRpc(player, randomSeed);

            isWaitingForClientAcknowledgments = true;
            clientAcknowledgments = 0;
            while (clientAcknowledgments < NetworkManager.Singleton.ConnectedClients.Count)
            {
                yield return null;
            }
            isWaitingForClientAcknowledgments = false;
        }
        public IEnumerator BuildDecks()
        {
            if (!IsServer) yield break;
            List<CardData> cardDataListPlayer1 = GenerateCardDataList(Player.Player1);
            List<CardData> cardDataListPlayer2 = GenerateCardDataList(Player.Player2);

            CardDataListWrapper wrapperPlayer1 = new()
            {
                CardDataList = cardDataListPlayer1
            };
            CardDataListWrapper wrapperPlayer2 = new()
            {
                CardDataList = cardDataListPlayer2
            };
            string serializedCardDataPlayer1 = JsonUtility.ToJson(wrapperPlayer1);
            string serializedCardDataPlayer2 = JsonUtility.ToJson(wrapperPlayer2);


            if (!IsHost)
            {
                BuildDeck(cardDataListPlayer1, Player.Player1);
                BuildDeck(cardDataListPlayer2, Player.Player2);
            }
            yield return SendBuildDecksRpcAndWait(Player.Player1, serializedCardDataPlayer1);
            yield return SendBuildDecksRpcAndWait(Player.Player2, serializedCardDataPlayer2);
        }

        private IEnumerator SendBuildDecksRpcAndWait(Player player, string serializedCardData)
        {
            isWaitingForClientAcknowledgments = true;
            clientAcknowledgments = 0;

            BuildDecksClientRpc(player, serializedCardData);

            while (clientAcknowledgments < NetworkManager.Singleton.ConnectedClients.Count)
            {
                yield return null;
            }

            isWaitingForClientAcknowledgments = false;
        }

        [ServerRpc(RequireOwnership = false)]
        private void AcknowledgeServerRpc()
        {
            if (!isWaitingForClientAcknowledgments) return;
            clientAcknowledgments++;
        }
        #endregion

        #region Client Side
        [ClientRpc]
        private void ShuffleDeckClientRpc(Player player, int seed)
        {
            Random.InitState(seed);
            GetPlayerDeck(player).Shuffle();
            AcknowledgeServerRpc();
        }
        [ClientRpc]
        public void BuildDecksClientRpc(Player player, string serializedCardData)
        {
            CardDataListWrapper wrapper = JsonUtility.FromJson<CardDataListWrapper>(serializedCardData);
            List<CardData> cardDataList = wrapper.CardDataList;
            BuildDeck(cardDataList, player);
            AcknowledgeServerRpc();
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
                    BuildCard(modifiedCardData, player);
                }

                _manaManager.InitializeManaFromDeck(deckController);
            }
        }

        private void BuildCard(CardData cardData, Player player)
        {
            var deckController = GetPlayerDeck(player);
            if (deckController == null) return;

            var cardController = _cardFactory.Create(cardData);
            deckController.AddCardToZone(cardController);
        }
        #endregion
        #endregion
    }
}
