using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Decks.Managers
{
    public interface IDeckManager
    {
        List<IDeckController> Decks { get; }
        void RemoveCard(Player player, ICardController card);
        IEnumerator ShuffleDeck(Player player);
        IEnumerator MoveCardToDeck(Player player, ICardController card, int index = 0, bool reverse = false);
        Dictionary<StoneType, int> GetDeckCost(Player player);
        IEnumerator LoadDecks();
        ICardController PeekCard(Player player, int index);
    }
}
