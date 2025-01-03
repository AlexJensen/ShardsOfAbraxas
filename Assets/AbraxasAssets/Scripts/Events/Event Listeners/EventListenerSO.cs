using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Stones.Triggers;
using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.EventListeners
{
    [CreateAssetMenu(menuName = "Abraxas/Data/EventListener")]
    public class EventListenerSO : ScriptableObject, IGameEventListener<IEvent>
    {
        #region Fields
        [SerializeField]
        private bool _isActive = true;
        [SerializeField, ReadOnly]
        private string _eventTypeName;

        
        #endregion

        #region Properties
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public Type EventType
        {
            get => string.IsNullOrEmpty(_eventTypeName) ? null : Type.GetType(_eventTypeName);
            set => _eventTypeName = value.AssemblyQualifiedName;
        }

        #endregion

        #region Dependencies
        protected TriggerStone StoneController;
        protected IEventManager EventManager;

        [Inject]
        public void Construct(TriggerStone stoneController, IEventManager eventManager)
        {
            StoneController = stoneController;
            EventManager = eventManager;

            InitializeListener();
        }

        internal void Initialize(Type iEventType)
        {
            EventType = iEventType;
        }

        public void InitializeListener()
        {
            if (IsActive)
            {
                EventManager.AddListener(this);
            }
        }

        public void RemoveListener()
        {
            if (IsActive)
            {
                EventManager.RemoveListener(this);
            }
        }
        #endregion

        #region IGameEventListener Implementation
        public bool ShouldReceiveEvent(IEvent eventData)
        {
            if (EventType == null) return false;
            return eventData != null && eventData.GetType() == EventType;
        }

        public IEnumerator OnEventRaised(IEvent eventData)
        {
            if (StoneController.CheckConditions())
            {
                yield return StoneController.InvokeTrigger();
            }
        }

        
        #endregion
    }
}
