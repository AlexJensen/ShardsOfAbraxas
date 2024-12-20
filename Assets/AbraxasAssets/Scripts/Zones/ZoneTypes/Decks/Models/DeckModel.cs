using Abraxas.Random.Managers;
using Abraxas.Zones.Models;
using System.Collections;

namespace Abraxas.Zones.Decks.Models
{

    class DeckModel : ZoneModel, IDeckModel
    {
        public DeckModel(IRandomManager randomManager) : base(randomManager)
        {
        }

        public override ZoneType Type => ZoneType.Deck;

        public override void Shuffle()
        {
            foreach (var card in CardList)
            {
                card.Hidden = true;
            }
            base.Shuffle();
        }
    }
}
