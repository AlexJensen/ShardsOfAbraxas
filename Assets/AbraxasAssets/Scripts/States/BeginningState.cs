using Abraxas.Behaviours.Events;
using Abraxas.Behaviours.Game;
using System.Collections;

namespace Abraxas.Scripts.States
{
    public class BeginningState : State
    {
        public BeginningState(GameManager game) : base(game)
        {
        }

        public override State NextState()
        {
            return new BeforeCombatState(game);
        }

        public override IEnumerator OnEnterState()
        {
            EventManager.Instance.BeginningStateStarted();
            GameManager.Instance.GenerateManaForActivePlayer();
            yield return GameManager.Instance.DrawCardsForActivePlayer(1);
            game.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield break;
        }
    }
}