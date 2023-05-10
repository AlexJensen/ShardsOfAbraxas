using Abraxas.Cards;
using Abraxas.Cards.Controllers;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Graveyards
{
    public interface IGraveyardManager
    {
        IEnumerator MoveCardToGraveyard(Player player, ICardController card);
        void AddCard(Player player, ICardController card);
    }
}
