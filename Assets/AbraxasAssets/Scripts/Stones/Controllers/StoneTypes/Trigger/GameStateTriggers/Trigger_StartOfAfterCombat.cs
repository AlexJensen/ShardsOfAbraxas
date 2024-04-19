using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;

namespace Abraxas.Stones.Controllers
{
    public class Trigger_StartOfAfterCombat : Trigger_StartOfState
    {
        public override IEnumerator OnEventRaised(GameStateEnteredEvent eventData)
        {
            yield return InvokeTrigger();
        }

        public override bool ShouldReceiveEvent(GameStateEnteredEvent eventData)
        {
            return eventData.State is AfterCombatState && Card.Zone is FieldController;
        }
    }
}
