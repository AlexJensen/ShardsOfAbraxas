using Abraxas.Events;
using Abraxas.Stones.Controllers;
using System;
using UnityEngine;
using Zenject;


namespace Abraxas.Stones.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/StartOfState")]
    [Serializable]
    public class Condition_IsCurrentState : ConditionSO<Event_GameStateEntered>
    {

        #region Dependencies
        public override void Initialize(IStoneController stone, ICondition condition, DiContainer container)
        {
            State = ((Condition_IsCurrentState)condition).State;
            base.Initialize(stone, condition, container);
        }
        #endregion

        #region Fields
        [SerializeField]
        GameStates.GameStates _state;
        GameStates.GameStates _currentState;

        public GameStates.GameStates State { get => _state;
            set => _state = value; }
        #endregion

        #region Methods
        public override bool IsMet()
        {
            return _currentState == State;
        }

        public override bool ShouldReceiveEvent(Event_GameStateEntered eventData)
        {
            _currentState = eventData.State.CurrentState;
            return IsMet();
        }
        #endregion
    }
}
