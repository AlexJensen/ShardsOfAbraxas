using Abraxas.Cards.Controllers;
using Abraxas.Events;
using System.Collections;
using UnityEngine;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/StartOfState")]
    public class StartOfStateCondition : Condition<GameStateEnteredEvent>,
        IGameEventListener<GameStateEnteredEvent>
    {
        [SerializeField]
        GameStates.GameStates _state;

        public GameStates.GameStates State { get => _state; set => _state = value; }

        public override void SubscribeToEvents()
        {
            if (IsTrigger)
            {
                EventManager.AddListener(typeof(GameStateEnteredEvent), this);
            }
        }

        public override void UnsubscribeFromEvents()
        {
            if (IsTrigger)
            {
                EventManager.RemoveListener(typeof(GameStateEnteredEvent), this);
            }
        }

        public override bool IsMet(ICardController card, GameStateEnteredEvent eventData)
        {
            return eventData.State.CurrentState == State;
        }

        public IEnumerator OnEventRaised(GameStateEnteredEvent eventData)
        {
            yield return NotifyTriggerStone();
        }

        public bool ShouldReceiveEvent(GameStateEnteredEvent eventData)
        {
            return eventData.State.CurrentState == State;
        }
    }
}
