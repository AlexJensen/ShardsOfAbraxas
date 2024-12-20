using Abraxas.AI.Managers;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using Abraxas.Manas;
using Abraxas.Random.Managers;
using Abraxas.Zones.Decks.Managers;
using System.Collections;
using Unity.Netcode;
using Zenject;

using Player = Abraxas.Players.Players;
namespace Abraxas.GameStates
{
    /// <summary>
    /// GameNotStartedState is a state that represents the game before it has started and is used to wait until both players have connected.
    /// </summary>
    public class GameNotStartedState : GameState
    {
        #region Dependencies
        readonly NetworkManager _networkManager;
        readonly IGameStateManager _gameStateManager;
        readonly IDeckManager _deckManager;
        readonly IManaManager _manaManager;
        readonly IRandomManager _randomManager;
        readonly IAIManager _aiManager;
        [Inject]
        public GameNotStartedState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager, IDeckManager deckManager, IManaManager manaManager, IRandomManager randomManager, IAIManager aiManager) : base(gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
            _networkManager = NetworkManager.Singleton;
            _deckManager = deckManager;
            _manaManager = manaManager;
            _randomManager = randomManager;
            _aiManager = aiManager;
        }

        public class Factory : PlaceholderFactory<GameNotStartedState>
        {
        }
        #endregion

        #region Properties
        public override GameStates CurrentState => GameStates.GameNotStarted;
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.Beginning;
        }

        public override IEnumerator OnEnterState()
        {

            yield return base.OnEnterState();
            if (_networkManager.IsHost )
            {
                yield return _randomManager.InitializeRandomSeed();
                yield return _deckManager.LoadDecks();
                yield return _deckManager.ShuffleDeck(Player.Player1);
                yield return _deckManager.ShuffleDeck(Player.Player2);
                yield return _gameStateManager.BeginNextGameState();
            }
            else if (_networkManager.IsServer)
            {
                while (_networkManager.ConnectedClients.Count != ((_aiManager.IsPlayer1AI || _aiManager.IsPlayer2AI) ? 1: 2))
                {
                    yield return null;
                }
                yield return _randomManager.InitializeRandomSeed();
                yield return _deckManager.LoadDecks();
                yield return _deckManager.ShuffleDeck(Player.Player1);
                yield return _deckManager.ShuffleDeck(Player.Player2);
                yield return _gameStateManager.BeginNextGameState();
            }
        }

        public override IEnumerator OnExitState()
        {

            yield return base.OnExitState();

            _manaManager.InitializeManaFromDecks(_deckManager.Decks);
            yield return gameManager.StartGame();
        }
        #endregion
    }
}