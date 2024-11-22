using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Stones.Controllers;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.EventListeners
{
    [CreateAssetMenu(menuName = "Abraxas/Data/EventListener")]
    public abstract class EventListenerSO<T> : EventListenerSOBase, IGameEventListener<T> where T : IEvent
    {
        #region Fields
        [SerializeField]
        private bool _isActive = true;
        #endregion

        #region Properties
        public bool IsActive { get => _isActive; set => _isActive = value; }
        #endregion

        #region Dependencies
        protected IStoneController StoneController;
        protected IEventManager EventManager;

        [Inject]
        public void Construct(IStoneController stoneController, IEventManager eventManager)
        {
            StoneController = stoneController;
            EventManager = eventManager;
        }

        public override void InitializeListener()
        {
            if (IsActive)
            {
                EventManager.AddListener(this);
            }
        }

        public override void RemoveListener()
        {
            if (IsActive)
            {
                EventManager.RemoveListener(this);
            }
        }
        #endregion

        #region IGameEventListener Implementation
        public abstract bool ShouldReceiveEvent(T eventData);

        public abstract IEnumerator OnEventRaised(T eventData);
        #endregion
    }

    public abstract class EventListenerSOBase : ScriptableObject
    {
        public abstract void InitializeListener();
        public abstract void RemoveListener();
    }
}
