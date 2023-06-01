using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using Abraxas.Zones.Fields.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    public class CombatState : GameState
    {
        #region Dependency Injections
        readonly IGameStateManager _gameStateManager;
        readonly IFieldManager _fieldManager;
        [Inject]
        public CombatState(IGameManager gameManager, IGameStateManager gameStateManager, IFieldManager fieldManager, IEventManager eventManager) : base(gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
            _fieldManager = fieldManager;
        }
        public class Factory : PlaceholderFactory<CombatState>{}
        #endregion

        #region Properties
        public override GameStates CurrentState => GameStates.Combat;
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.AfterCombat;
        }

        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            yield return _fieldManager.StartCombat();
            yield return _gameStateManager.BeginNextGameState();

        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}