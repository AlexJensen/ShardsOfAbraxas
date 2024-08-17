using Abraxas.Cards.Controllers;

namespace Abraxas.Events
{

    public class Event_CardChangedZones : IEvent
    {
        #region Properties
        public ICardController Card { get; set; }
        #endregion

        #region Methods
        public Event_CardChangedZones(ICardController card)
        {
            Card = card;
        }
        #endregion
    }
}
