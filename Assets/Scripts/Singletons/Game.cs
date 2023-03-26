using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/* The Game singleton handles all higher level logic for a match, including handling the state transitions between each phase of play. 
 * Data should never be modified directly, it should always be encapsulated into developer friendly methods.*/
/// </summary>
public class Game : Singleton<Game>
{
    State currentState;

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
    List<Hand> hands;
    [SerializeField]
    List<Mana> mana;
    [SerializeField]
    StoneData stoneData;
    [SerializeField]
    PlayerData playerData;

    public Player ActivePlayer { get; private set; } = Player.Player1;

    protected void Start()
    {
        currentState = new GameNotStartedState(this);
        StartCoroutine(currentState.OnEnterState());
    }

    private IEnumerator SwitchToState(State state)
    {
        yield return StartCoroutine(currentState.OnExitState());
        currentState = state;
        GameStateChanged?.Invoke(currentState);
        yield return StartCoroutine(currentState.OnEnterState());
    }

    public IEnumerator StartGame()
    {
        foreach (Hand hand in hands)
        {
            hand.Deck.ShuffleServerRpc();
        }
        yield return Utilities.WaitForCoroutines(this,
                        hands[0].DrawCardsFromLibrary(5),
                        hands[1].DrawCardsFromLibrary(5));
    }

    public void BeginNextGameState()
    {
        StartCoroutine(SwitchToState(currentState.NextState()));
    }

    public Mana GetPlayerMana(Game.Player player)
    {
        return mana.Find(x => x.Player == ActivePlayer);
    }

    public Hand GetPlayerHand(Game.Player player)
    {
        return hands.Find(x => x.player == ActivePlayer);
    }

    public void SwitchActivePlayer()
    {
        ActivePlayer = ActivePlayer == Player.Player1 ? Player.Player2 : Player.Player1;
    }

    public void GenerateManaForActivePlayer()
    {
        Mana currentPlayerMana = GetPlayerMana(ActivePlayer);
        manaGain = Math.Min(MAX_MANA_GAIN, manaGain + MANA_GAIN_PER_TURN);
        currentPlayerMana.GenerateRatioMana(manaGain);
    }

    public IEnumerator DrawCardsForActivePlayer(int amount)
    {
        Hand currentPlayerHand = GetPlayerHand(ActivePlayer);
        yield return currentPlayerHand.DrawCardsFromLibrary(amount);
    }

    public StoneData.StoneDetails GetStoneDetails(StoneData.StoneType type)
    {
        return stoneData.stones.Find(x => x.type == type);
    }

    public PlayerData.PlayerDetails GetPlayerDetails(Player player)
    {
        return playerData.players.Find(x => x.player == player);
    }

    public bool CanPurchaseCard(Card card)
    {
        Mana currentPlayerMana = GetPlayerMana(card.Owner);
        foreach (KeyValuePair<StoneData.StoneType, int> cost in card.TotalCosts)
        {
            ManaType result = currentPlayerMana.ManaTypes.Find(x => x.Type == cost.Key);
            if (!result || result.Amount < cost.Value)
            {
                return false;
            }
        }
        return true;
    }

    public void PurchaseCard(Card card)
    {
        Mana currentPlayerMana = GetPlayerMana(card.Owner);
        foreach (KeyValuePair<StoneData.StoneType, int> cost in card.TotalCosts)
        {
            ManaType result = currentPlayerMana.ManaTypes.Find(x => x.Type == cost.Key);
            result.Amount -= cost.Value;
        }

    }
}
