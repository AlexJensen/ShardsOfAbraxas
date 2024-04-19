using Abraxas.Cards.Controllers;

namespace Abraxas.Events
{
	public class CardChangedZonesEvent
	{
        #region Properties
        public ICardController Card { get; set; }
        #endregion

        #region Methods
        public CardChangedZonesEvent(ICardController card)
        {
            Card = card;
        }
        #endregion
    }
}
