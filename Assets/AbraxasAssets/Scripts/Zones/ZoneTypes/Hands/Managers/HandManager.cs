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
        readonly List<IHandController> _hands = new();

        public ICardController CardDragging { get; set; }

        [Inject]
        void Construct(ZoneFactory<IHandView, HandController, HandModel> handFactory)
        {
            foreach (var handView in _handViews)
            {
                _hands.Add(handFactory.Create(handView));
            }
        }
        #endregion

        #region Methods
        public void RemoveCard(ICardController card)
        {
            GetPlayerHand(card.OriginalOwner).RemoveCard(card);
        }
        public IEnumerator MoveCardToHand(Player player, ICardController card)
        {
            card.Zone = GetPlayerHand(player);
            yield return card.Zone.MoveCardToZone(card);
        }
        public IEnumerator ReturnCardToHand(ICardController card)
        {
            IHandController playerHand = GetPlayerHand(card.OriginalOwner);
            yield return playerHand.MoveCardToZone(card, playerHand.CardPlaceholderSiblingIndex);
        }
        private IHandController GetPlayerHand(Player player)
        {
            return _hands.Find(x => x.Player == player);
        }
        #endregion
    }
}