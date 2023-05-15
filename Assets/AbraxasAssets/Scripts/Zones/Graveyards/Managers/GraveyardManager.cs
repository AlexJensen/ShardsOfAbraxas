using Abraxas.Cards.Controllers;
using Abraxas.Zones.Graveyard.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Graveyards.Managers
{
    public class GraveyardManager : MonoBehaviour, IGraveyardManager
    {
        #region Fields
        [SerializeField]
        List<IGraveyardController> _graveyards;
        #endregion

        public IEnumerator MoveCardToGraveyard(Player player, ICardController card)
        {
            yield return GetPlayerGraveyard(player).AddCard(card);
        }

        private IGraveyardController GetPlayerGraveyard(Player player)
        {
            return _graveyards.Find(x => x.Player == player);
        }
    }
}
