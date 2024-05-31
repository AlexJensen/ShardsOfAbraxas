using Abraxas.Cards.Controllers;
using Abraxas.Events.Managers;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    public abstract class Condition<T> : ScriptableObject, ICondition<T>
    {
        protected IStoneController StoneController;
        protected IEventManager EventManager;

        [SerializeField]
        public bool IsTrigger = false;

        [Inject]
        public virtual void Construct(IEventManager eventManager)
        {
            EventManager = eventManager;
        }

        public void Initialize(IStoneController stoneController)
        {
            StoneController = stoneController;
            SubscribeToEvents();
        }

        public virtual void SubscribeToEvents()
        {

        }

        public virtual void UnsubscribeFromEvents()
        {

        }

        public abstract bool IsMet(ICardController card, T eventData);

        protected IEnumerator NotifyTriggerStone()
        {
            yield return ((TriggerStone)StoneController).CheckConditions();
        }
    }
}