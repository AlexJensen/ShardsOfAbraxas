using ModestTree;

namespace Abraxas.Scripts.States
{
    public enum GameStates
    {
        GameNotStarted,
        Beginning,
        BeforeCombat,
        Combat,
        AfterCombat, 
        End,   
    }

    public class GameStateFactory
    {
        readonly GameNotStartedState.Factory _gameNotStartedFactory;
        readonly BeginningState.Factory _beginningFactory;
        readonly BeforeCombatState.Factory _beforeCombatFactory;
        readonly CombatState.Factory _combatFactory;
        readonly AfterCombatState.Factory _afterCombatFactory;
        readonly EndState.Factory _endFactory;


        public GameStateFactory(
            GameNotStartedState.Factory gameNotStartedFactory,
            BeginningState.Factory beginningFactory,
            BeforeCombatState.Factory beforeCombatFactory,
            CombatState.Factory combatFactory,
            AfterCombatState.Factory afterCombatFactory,
            EndState.Factory endFactory)
        {
            _gameNotStartedFactory = gameNotStartedFactory;
            _beginningFactory = beginningFactory;
            _beforeCombatFactory = beforeCombatFactory;
            _combatFactory = combatFactory;
            _afterCombatFactory = afterCombatFactory;
            _endFactory = endFactory;
        }

        public GameState CreateState(GameStates state)
        {
            switch (state)
            {
                case GameStates.GameNotStarted:
                {
                    return _gameNotStartedFactory.Create();
                }
                case GameStates.Beginning:
                    {
                        return _beginningFactory.Create();
                    }
                case GameStates.BeforeCombat:
                    {
                        return _beforeCombatFactory.Create();
                    }
                case GameStates.Combat:
                    {
                        return _combatFactory.Create();
                    }
                case GameStates.AfterCombat:
                    {
                        return _afterCombatFactory.Create();
                    }
                case GameStates.End:
                    {
                        return _endFactory.Create();
                    }
            }

            throw Assert.CreateException();
        }
    }
}
