using Abraxas.Behaviours.Events;
using Abraxas.Behaviours.Game;
using System.Collections;
using Zenject;

namespace Abraxas.Scripts.States
{
    public abstract class GameState
    {
        protected GameManager gameManager;
        public abstract IEnumerator OnEnterState();
        public abstract IEnumerator OnExitState();
        public abstract GameStates NextState();
    }
}