using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Game : Singleton<Game>
{
    private State currentState;

    public event Action<State> GameStateChanged;

    const int STARTING_MANA = 0;
    const int MANA_GAIN_PER_TURN = 2;
    const int MAX_MANA_GAIN = 20;
    int manaGain = STARTING_MANA;

    public enum Player
    {
        Player1,
        Player2,
        Neutral
    }

    [SerializeField]
    public List<Hand> hands;
    [SerializeField]
    public List<Mana> mana;
    public StoneData stoneData;

    public Player CurrentPlayer { get; set; } = Player.Player1;

    protected void Initialize()
    {
        currentState = new GameNotStartedState(this);
        StartCoroutine(currentState.OnEnterState());
    }

    protected override void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (client) =>
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Initialize();
            }
        };
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            Initialize();
        };
    }

    public IEnumerator SwitchToState(State state)
    {
        yield return StartCoroutine(currentState.OnExitState());
        currentState = state;
        GameStateChanged?.Invoke(currentState);
        yield return StartCoroutine(currentState.OnEnterState());
    }

    public void BeginNextGameState()
    {
        StartCoroutine(SwitchToState(currentState.NextState()));
    }

    public Mana GetCurrentPlayerMana()
    {
        return mana.Find(x => x.Player == CurrentPlayer);
    }

    public Hand GetCurrentPlayerHand()
    {
        return hands.Find(x => x.player == CurrentPlayer);
    }

    public void SwitchCurrentPlayer()
    {
        CurrentPlayer = CurrentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
    }

    public void GenerateManaForCurrentPlayer()
    {
        Mana currentPlayerMana = GetCurrentPlayerMana();
        manaGain = Math.Min(MAX_MANA_GAIN, manaGain + MANA_GAIN_PER_TURN);
        currentPlayerMana.GenerateRatioMana(manaGain);
    }

    public IEnumerator DrawCardsForCurrentPlayer(int amount)
    {
        Hand currentPlayerHand = GetCurrentPlayerHand();
        yield return currentPlayerHand.DrawCardFromLibrary();
    }
}
