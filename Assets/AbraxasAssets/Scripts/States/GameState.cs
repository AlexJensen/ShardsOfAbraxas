using Abraxas.Events;
using Abraxas.Game;
using System.Collections;

namespace Abraxas.GameStates
{
    public abstract class GameState
    {
        protected readonly IGameManager gameManager;
        protected readonly IEventManager eventManager;

        protected GameState(IGameManager gameManager, IEventManager eventManager)
        {
            this.gameManager = gameManager;
            this.eventManager = eventManager;
        }

        public virtual IEnumerator OnEnterState()
        {
            yield return eventManager.RaiseEvent(typeof(GameStateEnteredEvent), new GameStateEnteredEvent(this));
        }
        public virtual IEnumerator OnExitState()
        {
            yield return eventManager.RaiseEvent(typeof(GameStateExitedEvent), new GameStateExitedEvent(this));
        }
        public abstract GameStates NextState();
    }
}