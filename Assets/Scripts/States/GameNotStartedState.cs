using System.Collections;
using Unity.Netcode;
using UnityEngine;

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
            while (!networkManager.IsServer) yield return null;
            while (networkManager.ConnectedClients.Count != 2) yield return null;
        }
        game.BeginNextGameState();
    }

    public override IEnumerator OnExitState()
    {
        foreach (Hand hand in game.hands)
        {
            hand.Deck.Shuffle();
        }
        yield return Utilities.WaitForCoroutines(game,
                        game.hands[0].DrawCardFromLibrary(5),
                        game.hands[1].DrawCardFromLibrary(5));
    }
}
