using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Decks.Scriptable_Objects;
using Abraxas.Network.Managers;
using Abraxas.Random.Managers;
using Abraxas.Stones;
using Abraxas.Zones.Decks.Controllers;
using Abraxas.Zones.Decks.Models;
using Abraxas.Zones.Decks.Views;
using Abraxas.Zones.Factories;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Decks.Managers
{

    class DeckManager : NetworkedManager, IDeckManager
    {
        #region Dependencies
        CardController.Factory _cardFactory;
        ZoneFactory<IDeckView, DeckController, DeckModel> _deckFactory;
        [Inject]
        public void Construct(CardController.Factory cardFactory, ZoneFactory<IDeckView, DeckController, DeckModel> deckFactory)
        {
            _cardFactory = cardFactory;
            _deckFactory = deckFactory;
        }

        #endregion

        #region Fields
        [SerializeField]
        List<DeckView> _deckViews;

        [SerializeField]
        private DeckDataSO player1DeckData, player2DeckData;

        readonly List<IDeckController> _decks = new();

        readonly JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public List<IDeckController> Decks => _decks;
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

        public void RemoveCard(Player player, ICardController card)
        {
            GetPlayerDeck(player).RemoveCard(card);
        }
        public ICardController PeekCard(Player player, int index)
        {
            return GetPlayerDeck(player).PeekCard(index);
        }
        private IDeckController GetPlayerDeck(Player player)
        {
            return _decks.Find(x => x.Player == player);
        }
        #endregion

        #region Server Methods
        #region Server Side
        public IEnumerator ShuffleDeck(Player player)
        {
            if (!IsServer) yield break;
            if (!IsHost) GetPlayerDeck(player).Shuffle();
            ShuffleDeckClientRpc(player);
            yield return WaitForClients();
        }
        public IEnumerator LoadDecks()
        {
            if (!IsServer) yield break;
            yield return LoadDeck(Player.Player1);
            yield return LoadDeck(Player.Player2);
        }

        private IEnumerator LoadDeck(Player player)
        {
            List<CardData> cardDataList = (player == Player.Player1 ? player1DeckData : player2DeckData).cards;
            List<List<CardData>> batches = new();
            List<CardData> currentBatch = new();
            int batchSizeBytes = 0;

            if (!IsHost)
            {
                BuildDeck(player);
            }
            yield return SendInitializeBuildingDeckAndWait(player);

            foreach (CardData cardData in cardDataList)
            {
                string serializedCardData = JsonConvert.SerializeObject(cardData, _settings);
                byte[] cardDataBytes = System.Text.Encoding.UTF8.GetBytes(serializedCardData);


                if (batchSizeBytes + cardDataBytes.Length > 6144)
                {
                    batches.Add(new List<CardData>(currentBatch));
                    currentBatch.Clear();
                    batchSizeBytes = 0;
                }

                currentBatch.Add(cardData);
                batchSizeBytes += cardDataBytes.Length;
            }

            if (currentBatch.Count > 0)
            {
                batches.Add(currentBatch);
            }

            foreach (var batch in batches)
            {
                string serializedBatch = JsonConvert.SerializeObject(batch, _settings);
                if (!IsHost)
                {
                    foreach (var cardData in batch)
                    {
                        BuildCard(cardData, player);
                    }
                }
                yield return SendBuildCardsBatchRpcAndWait(player, serializedBatch);
            }
        }

        private IEnumerator SendInitializeBuildingDeckAndWait(Player player)
        {
            InitializeBuildingDeckClientRpc(player);
            yield return WaitForClients();
        }

        private IEnumerator SendBuildCardsBatchRpcAndWait(Player player, string serializedBatch)
        {
            BuildCardsBatchClientRpc(player, serializedBatch);
            yield return WaitForClients();
        }
        #endregion

        

		#region Client Side
		[ClientRpc]
		private void ShuffleDeckClientRpc(Player player)
		{
			GetPlayerDeck(player).Shuffle();
			AcknowledgeServerRpc();
		}

        [ClientRpc]
        private void InitializeBuildingDeckClientRpc(Player player)
        {
			BuildDeck(player);
			AcknowledgeServerRpc();
        }

        [ClientRpc]
        private void BuildCardsBatchClientRpc(Player player, string serializedBatch)
        {
            var cardsBatch = JsonConvert.DeserializeObject<List<CardData>>(serializedBatch, _settings);
            foreach (var cardData in cardsBatch)
            {
                BuildCard(cardData, player);
            }
            AcknowledgeServerRpc();
        }

        private void BuildDeck(Player player)
		{
			foreach (var deckView in _deckViews)
			{
				if (player != deckView.Player) continue;
                var deckController = _deckFactory.Create(deckView);
                _decks.Add(deckController);		
			}
		}

        private void BuildCard(CardData cardData, Player player)
        {
            var deckController = GetPlayerDeck(player);
            if (deckController == null) return;

            var modifiedCardData = new CardData()
            {
                Title = cardData.Title,
                Owner = player,
                OriginalOwner = player,
                Stones = new List<StoneWrapper>(cardData.Stones),
                StatBlock = cardData.StatBlock,

            };

            var cardController = _cardFactory.Create(modifiedCardData);
            deckController.AddCard(cardController);
        }



        #endregion
        #endregion
    }

}