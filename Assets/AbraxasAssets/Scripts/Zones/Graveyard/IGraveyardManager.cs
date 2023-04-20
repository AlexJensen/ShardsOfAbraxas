using Abraxas.Cards;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Graveyards
{
    public interface IGraveyardManager
    {
        IEnumerator MoveCardToGraveyard(Player player, Card card);
        void AddCard(Player player, Card card);
    }
}
