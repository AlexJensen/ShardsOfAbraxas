using Abraxas.Cards.Controllers;
using Abraxas.Stones.Controllers;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
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

        bool CanPurchaseCard(ICardController model);
        bool CanPurchaseStone(IStoneController stone);
        void CreateManaTypesFromDeck(IDeckController deck);
        IEnumerator GenerateRatioMana(int amount);
        void PurchaseCard(ICardController card);
        void PurchaseStone(IStoneController stone);
    }
}
