using Abraxas.Cards.Controllers;
using Abraxas.Core;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Graveyards
{
    public interface IGraveyardManager : IManager
    {
        IEnumerator MoveCardToGraveyard(Player player, ICardController card);
    }
}
