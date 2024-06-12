using Abraxas.Stones;
using Abraxas.Zones.Decks.Controllers;
using System;
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
        List<ManaType> ManaTypes { get; set; }
        int TotalDeckCost { get; set; }
        int StartOfTurnMana { get; set; }
        Dictionary<StoneType, int> DeckCosts { get; set; }

        event Action OnStartOfTurnManaChanged;
    }
}
