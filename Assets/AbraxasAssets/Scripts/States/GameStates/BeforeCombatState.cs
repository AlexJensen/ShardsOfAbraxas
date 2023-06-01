using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    public class BeforeCombatState : GameState
    {
        #region Dependencies
        [Inject]
        public BeforeCombatState(IGameManager gameManager, IEventManager eventManager) : base(gameManager, eventManager) {}

        public class Factory : PlaceholderFactory<BeforeCombatState>{}
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
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}