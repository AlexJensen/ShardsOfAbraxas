using Abraxas.Cards.Controllers;
using Abraxas.Manas.Models;
using Abraxas.Stones.Controllers;
using Abraxas.Zones.Decks.Controllers;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;
namespace Abraxas.Manas.Controllers
{

    /// <summary>
    /// IManaController is an interface for the ManaController class.
    /// </summary>

    public interface IManaController
    {
        Player Player { get; }
        List<ManaType> ManaTypes { get; }

        int StartOfTurnMana { get; set; }

        void ApplyGeneratedMana(ManaAmounts generatedManaAmounts);
        bool CanPurchaseCard(ICardController model);
        bool CanPurchaseStone(IStoneController stone);
        void CreateManaTypesFromDeck(IDeckController deck);
        ManaAmounts GenerateRatioMana(int amount);
        void OnDestroy();
        void PurchaseCard(ICardController card);
        void PurchaseStone(IStoneController stone);
    }
}
