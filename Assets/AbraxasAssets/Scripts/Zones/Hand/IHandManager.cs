using Abraxas.Cards;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands
{
    public interface IHandManager
    {
        public IEnumerator MoveCardToHand(Player player, Card card);
        public void RemoveCard(Player player, Card card);
    }
}
