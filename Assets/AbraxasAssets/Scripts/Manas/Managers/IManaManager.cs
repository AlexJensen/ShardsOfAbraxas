using Abraxas.Cards.Controllers;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;

namespace Abraxas.Manas
{
    public interface IManaManager
    {
        int StartOfTurnMana { get; }

        IEnumerator GenerateManaFromDeckRatio(Player player, int startOfTurnMana);
        bool CanPurchaseCard(ICardController card);
        void IncrementStartOfTurnManaAmount();
        void PurchaseCard(ICardController card);
        void InitializeManaFromDeck(IDeckController deck);
        void InitializeManaFromDecks(List<IDeckController> decks);
    }
}
