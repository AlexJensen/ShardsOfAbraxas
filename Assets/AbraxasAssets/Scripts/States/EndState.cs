using Abraxas.Behaviours.Game;
using System.Collections;

namespace Abraxas.Scripts.States
{
    public class EndState : State
    {
        public EndState(GameManager game) : base(game)
        {
        }

        public override State NextState()
        {
            return new BeginningState(game);
        }

        public override IEnumerator OnEnterState()
        {
            game.BeginNextGameState();
            yield break;
        }

        public override IEnumerator OnExitState()
        {
            GameManager.Instance.SwitchActivePlayer();
            yield break;
        }
    }
}