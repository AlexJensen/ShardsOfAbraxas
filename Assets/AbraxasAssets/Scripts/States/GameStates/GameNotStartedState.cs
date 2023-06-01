using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using Abraxas.Zones.Decks.Managers;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;
namespace Abraxas.GameStates
{
    public class GameNotStartedState : GameState
    {
        #region Dependencies
        readonly NetworkManager _networkManager;
        readonly IGameStateManager _gameStateManager;
        readonly IDeckManager _deckManager;
        [Inject]
        public GameNotStartedState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager, IDeckManager deckManager) : base(gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
            _networkManager = NetworkManager.Singleton;
            _deckManager = deckManager;
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
            if (_networkManager.IsHost)
            {
                yield return base.OnEnterState();
                yield return _gameStateManager.BeginNextGameState();
            }
            else if(_networkManager.IsServer)
            {
                Debug.Log($"GameNotStartedState OnEnterState {_networkManager.IsServer}");
                while (_networkManager.ConnectedClients.Count != 2)
                {
                    yield return null;
                }
                Debug.Log($"GameNotStartedState OnEnterState Clients Joined");
                yield return _deckManager.BuildDecks();
                yield return _deckManager.ShuffleDeck(Player.Player1);
                yield return _deckManager.ShuffleDeck(Player.Player2);
                yield return _gameStateManager.BeginNextGameState();
            }
        }

        public override IEnumerator OnExitState()
        {
            Debug.Log($"GameNotStartedState OnExitState {_networkManager.IsServer}");
            yield return base.OnExitState();
            yield return gameManager.StartGame();
        }
        #endregion
    }
}