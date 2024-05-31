using Abraxas.Random.Managers;
using Abraxas.Zones.Models;

namespace Abraxas.Zones.Decks.Models
{

    class DeckModel : ZoneModel, IDeckModel
    {
        public DeckModel(IRandomManager randomManager) : base(randomManager)
        {
        }
    }
}
