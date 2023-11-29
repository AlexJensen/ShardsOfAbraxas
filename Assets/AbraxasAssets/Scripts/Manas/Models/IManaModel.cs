using Abraxas.Zones.Decks.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abraxas.Manas.Models
{
    public interface IManaModel
    {
        Players.Players Player { get; set; }
        List<ManaType> ManaTypes { get;  }
        int TotalDeckCost { get; }

        void CreateManaTypesFromDeck(IDeckController deck);
        IEnumerator GenerateRatioMana(int amount);
    }
}
