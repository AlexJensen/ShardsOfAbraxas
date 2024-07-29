using Abraxas.Cards.Controllers;

namespace Abraxas.Events
{

    public class Event_CardChangedZones : IEvent<ICardController>
    {
        #region Properties
        public ICardController Data { get; set; }
        #endregion

        #region Methods
        public Event_CardChangedZones(ICardController card)
        {
            Data = card;
        }
        #endregion
    }
}
