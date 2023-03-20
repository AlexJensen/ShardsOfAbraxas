using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// State to wait in while players are connecting.
/// </summary>
public class GameNotStartedState : State
{
    private NetworkManager networkManager;
    public GameNotStartedState(Game game) : base(game)
    {
        networkManager = NetworkManager.Singleton;
    }
    public override State NextState()
    {
        return new BeginningState(game);
    }

    public override IEnumerator OnEnterState()
    {
        if (DebugNetworkManager.Instance.isDebugMode)
        {
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            while (!networkManager.IsServer)
            {
                Debug.Log(networkManager.IsServer);
                yield return null;
            }
            while (networkManager.ConnectedClients.Count != 2)
            {
                Debug.Log(networkManager.ConnectedClients.Count);
                yield return null;
            }
        }
        game.BeginNextGameState();
    }

    public override IEnumerator OnExitState()
    {
        yield return game.StartGame();
    }
}
