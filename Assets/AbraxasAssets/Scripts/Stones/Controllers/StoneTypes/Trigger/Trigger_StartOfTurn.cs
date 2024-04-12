using Abraxas.Events.Managers;
using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;

namespace Abraxas.Stones.Controllers
{
    public class Trigger_StartOfTurn : TriggerStone, IGameEventListener<GameStateEnteredEvent>
    {
        readonly IEventManager _eventManager;

        public Trigger_StartOfTurn(IEventManager eventManager) : base()
        {
            _eventManager = eventManager;
            _eventManager.AddListener(typeof(GameStateEnteredEvent), this);
        }


        ~Trigger_StartOfTurn()
        {
            _eventManager.RemoveListener(typeof(GameStateEnteredEvent), this);
        }

        public IEnumerator OnEventRaised(GameStateEnteredEvent eventData)
        {
            if (eventData.State is BeginningState && Card.Zone is FieldController)
            {
                yield return InvokeTrigger();
            }
        }
    }
}
