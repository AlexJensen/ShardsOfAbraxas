using Abraxas.Events;
using System;
using UnityEngine;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/StartOfState")]
    [Serializable]
    public class IsCurrentStateCondition : Condition<GameStateEnteredEvent>
    {

        #region Dependencies
        public override void Initialize(TriggerStone stone, ICondition condition)
        {
            State = ((IsCurrentStateCondition)condition).State;
            base.Initialize(stone, condition);
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

        public override bool ShouldReceiveEvent(GameStateEnteredEvent eventData)
        {
            _currentState = eventData.State.CurrentState;
            return base.ShouldReceiveEvent(eventData);
        }
        #endregion
    }
}
