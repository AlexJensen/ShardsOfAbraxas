using Abraxas.Behaviours.Game;
using System.Collections;

namespace Abraxas.Scripts.States
{
    public abstract class State
    {
        protected GameManager game;

        public State(GameManager game)
        {
            this.game = game;
        }

        public abstract IEnumerator OnEnterState();
        public abstract IEnumerator OnExitState();
        public abstract State NextState();
    }
}