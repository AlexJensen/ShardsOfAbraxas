using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using System.Collections;
using System.Collections.Generic;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Controllers
{
    public interface IZoneController
    {
        Player Player { get; }
        Dictionary<StoneType, int> GetTotalCostOfZone();
        ICardController RemoveCard(ICardController card);
        IEnumerator AddCard(ICardController card, int index = 0);
        ICardController RemoveCard(int index);

        void Shuffle();
    }
}
