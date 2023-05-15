using System.Collections;
using System.Collections.Generic;
using Abraxas.Cards.Controllers;
using Abraxas.Zones.Hands.Controllers;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands.Managers
{
    public class HandManager : MonoBehaviour, IHandManager
    {
        #region Fields
        [SerializeField]
        List<IHandController> _hands;
        #endregion

        #region Methods
        public void RemoveCard(Player player, ICardController card)
        {
            GetPlayerHand(player).RemoveCard(card);
        }

        public IEnumerator MoveCardToHand(Player player, ICardController card)
        {
            yield return GetPlayerHand(player).AddCard(card);
        }

        public IEnumerator ReturnCardToHand(ICardController card)
        {
            IHandController playerHand = GetPlayerHand(card.OriginalOwner);
            yield return playerHand.AddCard(card, playerHand.CardPlaceholderSiblingIndex);
        }

        private IHandController GetPlayerHand(Player player)
        {
            return _hands.Find(x => x.Player == player);
        }
        #endregion
    }
}
