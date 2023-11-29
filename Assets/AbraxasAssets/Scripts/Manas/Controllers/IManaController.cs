using Abraxas.Cards.Models;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;
namespace Abraxas.Manas.Controllers
{
    public interface IManaController
    {
        Player Player { get; }
        List<ManaType> ManaTypes { get; }

        bool CanPurchaseCard(ICardModel model);
        void CreateManaTypesFromDeck(IDeckController deck);
        IEnumerator GenerateRatioMana(int amount);
        void PurchaseCard(ICardModel model);
    }
}
