using System.Collections;
using System.Collections.Generic;
using Abraxas.Cards.Controllers;
using Abraxas.Zones.Factories;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Hands.Models;
using Abraxas.Zones.Hands.Views;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands.Managers
{
    public class HandManager : MonoBehaviour, IHandManager
    {
        #region Dependencies
        [SerializeField]
        List<HandView> _handViews;
        List<IHandController> _hands = new();
        [Inject]
        void Construct(ZoneFactory<IHandView, HandController, HandModel> handFactory)
        {
            foreach (var deckView in _handViews)
            {
                _hands.Add(handFactory.Create(deckView));
            }
        }
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
