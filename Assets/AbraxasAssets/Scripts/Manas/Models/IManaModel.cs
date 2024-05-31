using Abraxas.Zones.Decks.Controllers;

using System.Collections;
using System.Collections.Generic;

namespace Abraxas.Manas.Models
{
    /// <summary>
    /// IManaModel is an interface for the ManaModel class.
    /// </summary>
    public interface IManaModel
    {
        Players.Players Player { get; set; }
        List<ManaType> ManaTypes { get; }
        int TotalDeckCost { get; }
        int StartOfTurnMana { get; set; }

        void CreateManaTypesFromDeck(IDeckController deck);
        IEnumerator GenerateRatioMana(int amount);
    }
}
