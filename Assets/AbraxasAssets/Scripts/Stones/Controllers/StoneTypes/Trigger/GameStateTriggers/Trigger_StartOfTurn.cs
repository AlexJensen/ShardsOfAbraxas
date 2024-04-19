using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;

namespace Abraxas.Stones.Controllers
{
    public class Trigger_StartOfTurn : Trigger_StartOfState
    {
        public override IEnumerator OnEventRaised(GameStateEnteredEvent eventData)
        {
            yield return InvokeTrigger();
        }

        public override bool ShouldReceiveEvent(GameStateEnteredEvent eventData)
        {
            return eventData.State is BeginningState && Card.Zone is FieldController;
        }
    }
}
