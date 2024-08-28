using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Triggers;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Conditions
{
    public abstract class ConditionSOBase : ScriptableObject, ICondition 
    {
        public abstract void Initialize(IStoneController stoneController, ICondition condition, DiContainer container);
        public abstract bool IsMet();
        public abstract void SubscribeToEvents();
        public abstract void UnsubscribeFromEvents();

        public class Factory : PlaceholderFactory<ConditionSO<IEvent>, IStoneController, ICondition> { }
    }

    [Serializable]
    public abstract class ConditionSO<T> : ConditionSOBase, IGameEventListener<T>
        where T : IEvent
    {
        #region Fields
        protected IStoneController Stone;
        protected IEventManager EventManager;
        [SerializeField]
        private bool _isTrigger = false;
        #endregion

        #region Properties
        public virtual bool IsTrigger { get => _isTrigger; set => _isTrigger = value; }
        #endregion

        #region Methods
        public override void Initialize(IStoneController stone, ICondition condition, DiContainer container)
        {
            Stone = stone;
            IsTrigger = ((ConditionSO<T>)condition).IsTrigger;
            EventManager = container.Resolve<IEventManager>();
            SubscribeToEvents();
        }

        ~ConditionSO()
        {
            UnsubscribeFromEvents();
        }

        public override void SubscribeToEvents()
        {
            
            EventManager.AddListener(this);
        }

        public override void UnsubscribeFromEvents()
        {
            EventManager?.RemoveListener(this);
        }

        public virtual IEnumerator OnEventRaised(T eventData)
        {
            if (IsTrigger && Stone is TriggerStone)
            {
                InvokeTriggerIfConditionsMet();
            }
            yield return null;
        }

        protected void InvokeTriggerIfConditionsMet()
        {
            if ((Stone as TriggerStone).CheckConditions())
            {
                (Stone as TriggerStone).InvokeTrigger();
            }
        }

        public abstract bool ShouldReceiveEvent(T eventData);
        #endregion
    }
}
