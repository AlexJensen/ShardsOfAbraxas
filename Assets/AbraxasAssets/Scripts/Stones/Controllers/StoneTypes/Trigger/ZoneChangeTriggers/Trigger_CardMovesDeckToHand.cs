using Abraxas.Events;
using Abraxas.Zones.Decks.Controllers;
using Abraxas.Zones.Hands.Controllers;
using System.Collections;

namespace Abraxas.Stones.Controllers
{
	public class Trigger_CardMovesDeckToHand : TriggerStone, IGameEventListener<CardChangedZonesEvent>
	{
		public IEnumerator OnEventRaised(CardChangedZonesEvent eventData)
		{
            yield return InvokeTrigger();
		}

        public bool ShouldReceiveEvent(CardChangedZonesEvent eventData)
        {
            return eventData.Card.Zone is IHandController || eventData.Card.PreviousZone is IDeckController;
        }
    }
}
