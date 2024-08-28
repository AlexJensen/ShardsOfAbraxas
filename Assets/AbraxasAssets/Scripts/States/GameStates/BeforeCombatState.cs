using Abraxas.Cards.Managers;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    /// <summary>
    /// BeforeCombatState is the first main phase state before combat.
    /// </summary>
    public class BeforeCombatState : GameState
    {
        #region Dependencies
        
        readonly IGameStateManager _gameStateManager;
        readonly ICardManager _cardManager;

        [Inject]
        public BeforeCombatState(IGameManager gameManager, IGameStateManager gameStateManager, ICardManager cardManager, IEventManager eventManager) : base(gameManager, eventManager) 
        {
            _gameStateManager = gameStateManager;
            _cardManager = cardManager;
        }

        public class Factory : PlaceholderFactory<BeforeCombatState> { }
        #endregion

        #region Properties
        public override GameStates CurrentState => GameStates.BeforeCombat;
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.Combat;
        }

        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            bool continueState = true;
            foreach (var card in _cardManager.Cards)
            {
                if (card.DeterminePlayability())
                {
                    continueState = false;
                }
            }
            if (continueState)
            {
                yield return _gameStateManager.BeginNextGameState();
            }
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}