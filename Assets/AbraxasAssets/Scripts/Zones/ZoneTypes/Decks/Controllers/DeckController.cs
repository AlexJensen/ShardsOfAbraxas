using Abraxas.Zones.Controllers;
using Abraxas.Zones.Decks.Models;
using Abraxas.Zones.Decks.Views;
using Zenject;

namespace Abraxas.Zones.Decks.Controllers
{
    class DeckController : ZoneController, IDeckController
    {
        public class Factory : PlaceholderFactory<IDeckView, IDeckController, IDeckModel>
        {

        }

        public virtual void Shuffle()
        {
            Model.Shuffle();
        }
    }
}
