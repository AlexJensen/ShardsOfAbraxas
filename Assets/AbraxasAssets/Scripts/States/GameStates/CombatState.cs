using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Zones.Fields;
using System.Collections;
using Zenject;

namespace Abraxas.Scripts.States
{
    public class CombatState : GameState
    {
        #region Dependency Injections
        readonly FieldManager _fieldManager;
        public class Factory : PlaceholderFactory<CombatState>
        {
        }

        #endregion

        public CombatState(GameManager gameManager, FieldManager fieldManager)
        {
            this.gameManager = gameManager;
            _fieldManager = fieldManager;
        }



        public override GameStates NextState()
        {
            return GameStates.AfterCombat;
        }

        public override IEnumerator OnEnterState()
        {
            yield return _fieldManager.StartCombat();
            gameManager.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield break;
        }
    }
}