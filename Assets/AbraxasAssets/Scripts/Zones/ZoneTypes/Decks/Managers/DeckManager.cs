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
        public void BuildDecks()
        {
            foreach (var deckView in _deckViews)
            {
                IDeckController deckController = _deckFactory.Create(deckView);
                _decks.Add(deckController);
                for (int i = 0; i < 10; i++)
                {
                    int cost = Random.Range(1, 50);
                    StoneType type = (StoneType)Random.Range(0, 11);
                    int atk = Random.Range(1, 5);
                    int def = Random.Range(1, 5);
                    int mv = Random.Range(1, 5);
                    for (int j = 0; j < 4; j++)
                    {
                        CardData cardData = new()
                        {
                            Title = i.ToString(),
                            Stones =
                        {

                        },
                            StatBlock = new StatBlockData
                            {
                                Cost = cost,

                                StoneType = type,
                                [StatValues.ATK] = atk,
                                [StatValues.DEF] = def,
                                [StatValues.MV] = mv
                            }
                        };

                        ICardController cardController = _cardFactory.Create(cardData, deckController.Player);
                        deckController.AddCardToZone(cardController);
                    }
                }
                _manaManager.InitializeManaFromDeck(deckController);
            }
        }
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
            Debug.Log($"DeckManagerRemoveCard: {player}:{index}");
            return GetPlayerDeck(player).RemoveCard(index);
        }
        public IEnumerator ShuffleDeck(Player player)
        {
            ShuffleDeckServerRpc(player);
            yield return new WaitForSeconds(.1f);
        }
        private IDeckController GetPlayerDeck(Player player)
        {
            return _decks.Find(x => x.Player == player);
        }
        #endregion

        #region Server Methods
        [ServerRpc(RequireOwnership = false)]
        private void ShuffleDeckServerRpc(Player player)
        {
            int randomSeed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(randomSeed);
            GetPlayerDeck(player).Shuffle();
            ShuffleDeckClientRpc(player, randomSeed);
        }
        [ClientRpc]
        private void ShuffleDeckClientRpc(Player player, int seed)
        {
            Random.InitState(seed);
            GetPlayerDeck(player).Shuffle();
        }
        #endregion
    }
}
