using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Decks.Scriptable_Objects;
using Abraxas.Manas;
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
using Random = UnityEngine.Random;

namespace Abraxas.Zones.Decks.Managers
{
    class DeckManager : NetworkBehaviour, IDeckManager
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

        private int clientAcknowledgments = 0;
		private bool isWaitingForClientAcknowledgments = false;

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
		public ICardController RemoveCard(Player player, int index)
		{
			return GetPlayerDeck(player).RemoveCard(index);
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
			int randomSeed = Random.Range(int.MinValue, int.MaxValue);
			Random.InitState(randomSeed);
			if (!IsHost) GetPlayerDeck(player).Shuffle();
			ShuffleDeckClientRpc(player, randomSeed);

			isWaitingForClientAcknowledgments = true;
			clientAcknowledgments = 0;
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
			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
			};

            List<CardData> cardDataList = (player == Player.Player1 ? player1DeckData : player2DeckData).cards;

			if (!IsHost)
			{
				BuildDeck(player);
            }
			yield return SendInitializeBuildingDeckAndWait(player);

			foreach (CardData cardData in cardDataList)
			{
                string serializedCardData = JsonConvert.SerializeObject(cardData, settings);

                if (!IsHost)
                {
                    BuildCard(cardData, player);
                }

                yield return SendBuildCardRpcAndWait(player,serializedCardData);
            }
		}

        private IEnumerator SendInitializeBuildingDeckAndWait(Player player)
        {
			InitializeBuildingDeckClientRpc(player);
            yield return WaitForClients();
        }



        private IEnumerator SendBuildCardRpcAndWait(Player player, string serializedCardData)
		{
			BuildCardClientRpc(player, serializedCardData);
			yield return WaitForClients();
		}

		private IEnumerator WaitForClients()
		{
			isWaitingForClientAcknowledgments = true;
			clientAcknowledgments = 0;
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
        private void InitializeBuildingDeckClientRpc(Player player)
        {
			BuildDeck(player);
			AcknowledgeServerRpc();
        }

		[ClientRpc]
		private void BuildCardClientRpc(Player player, string serializedCardData)
		{
			var card = (CardData)JsonConvert.DeserializeObject(serializedCardData);
			BuildCard(card, player);
            AcknowledgeServerRpc();
        }

        private void BuildDeck(Player player)
		{
			foreach (var deckView in _deckViews)
			{
				var deckController = _deckFactory.Create(deckView);
				if (player != deckController.Player) continue;
				_decks.Add(deckController);		
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