using Abraxas.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Graveyards
{
    public class GraveyardManager : MonoBehaviour, IGraveyardManager
    {
        #region Fields
        [SerializeField]
        List<Graveyard> _graveyards;
        #endregion

        public void AddCard(Player player, Card card)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator MoveCardToGraveyard(Player player, Card card)
        {
            yield return GetPlayerGraveyard(player).MoveCardToZone(card);
        }

        private Graveyard GetPlayerGraveyard(Player player)
        {
            return _graveyards.Find(x => x.Player == player);
        }
    }
}