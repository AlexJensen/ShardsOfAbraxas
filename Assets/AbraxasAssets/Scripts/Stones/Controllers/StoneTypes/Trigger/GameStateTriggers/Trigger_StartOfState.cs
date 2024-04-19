using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;
using Zenject;

namespace Abraxas.Stones.Controllers
{
    public abstract class Trigger_StartOfState : TriggerStone,
        IGameEventListener<GameStateEnteredEvent>,
		IGameEventListener<CardChangedZonesEvent>
	{
        IEventManager _eventManager;

        [Inject]
        public void Construct(IEventManager eventManager)
        {
            _eventManager = eventManager;
            _eventManager.AddListener(typeof(CardChangedZonesEvent), this as IGameEventListener<CardChangedZonesEvent>);
        }

        ~Trigger_StartOfState()
        {
            _eventManager.RemoveListener(typeof(CardChangedZonesEvent), this as IGameEventListener<CardChangedZonesEvent>);
        }

        public abstract IEnumerator OnEventRaised(GameStateEnteredEvent eventData);

		public IEnumerator OnEventRaised(CardChangedZonesEvent eventData)
		{
            if (eventData.Card.Equals(Card))
            {
                if (Card.Zone is FieldController)
                {
                    _eventManager.AddListener(typeof(GameStateEnteredEvent), this as IGameEventListener<GameStateEnteredEvent>);
                    yield break;
                }
                else if (Card.PreviousZone is FieldController)
                {
                    _eventManager.RemoveListener(typeof(GameStateEnteredEvent), this as IGameEventListener<GameStateEnteredEvent>);
                    yield break;
                }
            }
		}

        public bool ShouldReceiveEvent(CardChangedZonesEvent eventData)
        {
            return Card.Zone is FieldController || Card.PreviousZone is FieldController;
        }

        public abstract bool ShouldReceiveEvent(GameStateEnteredEvent eventData);
    }
}
