using Abraxas.Cards.Controllers;
using Abraxas.Zones.Factories;
using Abraxas.Zones.Graveyard.Controllers;
using Abraxas.Zones.Graveyards.Models;
using Abraxas.Zones.Graveyards.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Graveyards.Managers
{
    public class GraveyardManager : MonoBehaviour, IGraveyardManager
    {
        #region Dependencies
        [SerializeField]
        List<GraveyardView> _graveyardViews;
        List<IGraveyardController> _graveyards = new();
        [Inject]
        void Construct(ZoneFactory<IGraveyardView, GraveyardController, GraveyardModel> graveyardFactory)
        {
            foreach (var deckView in _graveyardViews)
            {
                _graveyards.Add(graveyardFactory.Create(deckView));
            }
        }
        #endregion

        #region Methods
        public IEnumerator MoveCardToGraveyard(Player player, ICardController card)
        {
            yield return GetPlayerGraveyard(player).MoveCardToZone(card);
        }

        private IGraveyardController GetPlayerGraveyard(Player player)
        {
            return _graveyards.Find(x => x.Player == player);
        }
        #endregion
    }
}
