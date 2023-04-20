using Abraxas.Cards;
using Abraxas.Players;
using Abraxas.Stones;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Decks
{
    public class DeckManager : MonoBehaviour, IDeckManager
    {
        #region Fields
        [SerializeField]
        List<Deck> _decks;
        #endregion

        #region Methods
        public void AddCard(Card card)
        {
            GetPlayerDeck(card.Owner).AddCard(card);
        }

        public Dictionary<StoneType, int> GetDeckCost(Player player)
        {
            return GetPlayerDeck(player).GetDeckCost();
        }

        public IEnumerator MoveCardToDeck(Player player, Card card)
        {
            yield return GetPlayerDeck(player).MoveCardToZone(card);
        }

        public Card RemoveCard(Player player, int index)
        {
            return GetPlayerDeck(player).RemoveCard(index);
        }
        
        public IEnumerator ShuffleDeck(Player player)
        {
            GetPlayerDeck(player).ShuffleServerRpc();
            yield break;
        }

        private Deck GetPlayerDeck(Player player)
        {
            return _decks.Find(x => x.Player == player);
        }
        #endregion
    }
}
