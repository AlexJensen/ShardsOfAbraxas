﻿using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Triggers;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Conditions
{
    [Serializable]
    public abstract class ConditionSO<T> : ScriptableObject, ICondition, IGameEventListener<T>
        where T : class
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
        [Inject]
        public virtual void Construct(IEventManager eventManager)
        {
            EventManager = eventManager;
        }

        public virtual void Initialize(IStoneController stone, ICondition condition)
        {
            Stone = stone;
            IsTrigger = ((ConditionSO<T>)condition).IsTrigger;
            SubscribeToEvents();
        }

        ~ConditionSO()
        {
            UnsubscribeFromEvents();
        }

        public void SubscribeToEvents()
        {
            EventManager.AddListener(this);
        }

        public void UnsubscribeFromEvents()
        {
            EventManager.RemoveListener(this);
        }

        public abstract bool IsMet();

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