using System.Collections;
using UnityEngine;

public abstract class State
{
    protected Game game;

    public State(Game game)
    {
        this.game = game;
    }

    public abstract IEnumerator OnEnterState();
    public abstract IEnumerator OnExitState();
    public abstract State NextState();
}
