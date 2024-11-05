using Abraxas.Cards.Controllers;
using Abraxas.Stones.Controllers;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;

namespace Abraxas.Manas
{
    /// <summary>
    /// IManaManager is an interface for the ManaManager class.
    /// </summary>
    public interface IManaManager
    {
        IEnumerator GenerateManaFromDeckRatio(Player player, int startOfTurnMana);
        bool CanPurchaseCard(ICardController card);
        void PurchaseCard(ICardController card);
        bool CanPurchaseStoneActivation(IStoneController stone);
        void PurchaseStoneActivation(IStoneController stone);
        void IncrementStartOfTurnManaAmount(Player player);
        void InitializeManaFromDeck(IDeckController deck);
        void InitializeManaFromDecks(List<IDeckController> decks);
        int GetStartOfTurnMana(Player player);
    }
}
