using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Decks.Managers
{
    public class DeckManager : NetworkBehaviour, IDeckManager
    {
        #region Fields
        [SerializeField]
        List<IDeckController> _decks;
        #endregion

        #region Methods
        public void AddCard(ICardController card)
        {
            GetPlayerDeck(card.OriginalOwner).AddCard(card);
        }

        public Dictionary<StoneType, int> GetDeckCost(Player player)
        {
            return GetPlayerDeck(player).GetTotalCostOfZone();
        }

        public IEnumerator MoveCardToDeck(Player player, ICardController card)
        {
            yield return GetPlayerDeck(player).AddCard(card);
        }

        public ICardController RemoveCard(Player player, int index)
        {
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
