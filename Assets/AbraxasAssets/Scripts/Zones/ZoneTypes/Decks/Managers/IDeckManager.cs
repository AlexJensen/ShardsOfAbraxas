using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using System.Collections;
using System.Collections.Generic;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Decks.Managers
{
    public interface IDeckManager
    {
        ICardController RemoveCard(Player player, int index);
        IEnumerator ShuffleDeck(Player player);
        IEnumerator MoveCardToDeck(Player player, ICardController card);
        Dictionary<StoneType, int> GetDeckCost(Player player);
        IEnumerator BuildDecks();
    }
}
