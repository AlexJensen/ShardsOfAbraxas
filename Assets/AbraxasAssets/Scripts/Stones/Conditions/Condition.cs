using Abraxas.Events;
using Abraxas.Events.Managers;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [Serializable]
    public abstract class Condition<T> : ScriptableObject, ICondition, IGameEventListener<T>
        where T : IEvent
    {
        protected IStoneController Stone;
        protected IEventManager EventManager;

        public bool IsTrigger = false;

        [Inject]
        public virtual void Construct(IEventManager eventManager)
        {
            EventManager = eventManager;
        }

        public virtual void Initialize(TriggerStone stone, ICondition condition)
        {
            Stone = stone;
            IsTrigger = ((Condition<T>)condition).IsTrigger;
            SubscribeToEvents();
            stone.Conditions.Add(this);
        }

        ~Condition()
        {
            UnsubscribeFromEvents();
        }

        public void SubscribeToEvents()
        {
            EventManager.AddListener(typeof(T), this);
        }

        public void UnsubscribeFromEvents()
        {
            EventManager.RemoveListener(typeof(T), this);
        }

        public abstract bool IsMet();

        public IEnumerator OnEventRaised(T eventData)
        {
           if (IsTrigger) yield return ((TriggerStone)Stone).CheckConditions();
        }

        public virtual bool ShouldReceiveEvent(T eventData)
        {
            return IsMet();
        }
    }
}