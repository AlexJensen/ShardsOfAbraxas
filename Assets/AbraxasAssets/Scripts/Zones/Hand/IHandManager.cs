using Abraxas.Cards;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands
{
    public interface IHandManager
    {
        IEnumerator MoveCardToHand(Player player, Card card);
        void RemoveCard(Player player, Card card);
        IEnumerator ReturnCardToHand(Card card);
    }
}
