using System.Collections;
using System.Collections.Generic;
using Abraxas.Cards.Controllers;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands
{
    public class HandManager : MonoBehaviour, IHandManager
    {
        #region Fields
        [SerializeField]
        List<HandView> _hands;
        #endregion

        #region Methods
        public void RemoveCard(Player player, ICardController card)
        {
            GetPlayerHand(player).RemoveCard(card);
        }

        public IEnumerator MoveCardToHand(Player player, ICardController card)
        {
            yield return GetPlayerHand(player).MoveCardToZone(card);
        }

        public IEnumerator ReturnCardToHand(ICardController card)
        {
            HandView playerHand = GetPlayerHand(card.OriginalOwner);
            yield return playerHand.MoveCardToZone(card, playerHand.CardPlaceholder.transform.GetSiblingIndex());
        }

        private HandView GetPlayerHand(Player player)
        {
            return _hands.Find(x => x.Player == player);
        }
        #endregion
    }
}
