using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using System.Collections;

namespace Abraxas.GameStates
{
    /// <summary>
    /// GameState is an abstract class for managing game states.
    /// </summary>
    public abstract class GameState
    {
        #region Dependencies
        protected readonly IGameManager gameManager;
        protected readonly IEventManager eventManager;
        protected GameState(IGameManager gameManager, IEventManager eventManager)
        {
            this.gameManager = gameManager;
            this.eventManager = eventManager;
        }
        #endregion

        #region Properties
        public virtual GameStates CurrentState { get; }
        #endregion

        #region Methods
        public virtual IEnumerator OnEnterState()
        {
            yield return eventManager.RaiseEvent(typeof(GameStateEnteredEvent), new GameStateEnteredEvent(this));
        }
        public virtual IEnumerator OnExitState()
        {
            yield return eventManager.RaiseEvent(typeof(GameStateExitedEvent), new GameStateExitedEvent(this));
        }
        public abstract GameStates NextState();
        #endregion
    }
}