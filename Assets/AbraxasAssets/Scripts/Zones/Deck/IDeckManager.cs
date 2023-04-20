using Abraxas.Cards;
using Abraxas.Stones;
using System.Collections;
using System.Collections.Generic;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Decks
{
    public interface IDeckManager
    {
        Card RemoveCard(Player player, int index);
        Dictionary<StoneType, int> GetDeckManaRatio(Player player);
        IEnumerator ShuffleDeck(Player player);
        IEnumerator MoveCardToDeck(Player player, Card card);
        void AddCard(Card card);
    }
}
