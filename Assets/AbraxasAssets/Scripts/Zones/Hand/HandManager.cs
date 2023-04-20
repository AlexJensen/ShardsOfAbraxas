using System.Collections;
using System.Collections.Generic;
using Abraxas.Cards;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands
{
    public class HandManager : MonoBehaviour, IHandManager
    {
        #region Fields
        [SerializeField]
        List<Hand> _hands;
        #endregion

        #region Methods
        public void RemoveCard(Player player, Card card)
        {
            GetPlayerHand(player).RemoveCard(card);
        }
        public IEnumerator MoveCardToHand(Player player, Card card)
        {
            yield return GetPlayerHand(player).MoveCardToZone(card);
        }
        private Hand GetPlayerHand(Player player)
        {
            return _hands.Find(x => x.Player == player);
        }
        #endregion
    }
}
