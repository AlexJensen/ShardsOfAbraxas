using Abraxas.Cards.Controllers;
using Abraxas.Stones.Controllers;
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
        void PurchaseCard(ICardController card);
        bool CanPurchaseStoneActivation(IStoneController stone);
        void PuchaseStoneActivation(IStoneController stone);
        void IncrementStartOfTurnManaAmount();
        void InitializeManaFromDeck(IDeckController deck);
        void InitializeManaFromDecks(List<IDeckController> decks);
    }
}
