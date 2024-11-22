using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Stones.Triggers;
using System;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Data/StoneData/EventListener")]
    public class EventListener : ScriptableObject
    {
        [SerializeField]
        private string _eventType;

        private TriggerStone _triggerStone;

        public void Initialize(TriggerStone triggerStone)
        {
            _triggerStone = triggerStone;
            SubscribeToEvent();
        }

        private void SubscribeToEvent()
        {
            var eventType = Type.GetType(_eventType);
            if (eventType == typeof(Event_ActivePlayerChanged))
            {

            }
            // Add more event type subscriptions as needed.
        }

        private void OnEventTriggered(IEvent eventData)
        {

        }
    }
}
