using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abraxas.Core;
using Abraxas.Scripts.States;
using Abraxas.Behaviours.Zones.Hands;
using Abraxas.Behaviours.Manas;
using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Player;

namespace Abraxas.Behaviours.Game
{

    /// <summary>
    /* The Game singleton handles all higher level logic for a match, including handling the state transitions between each phase of play. */
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public State currentState;

        public event Action<State> GameStateChanged;

        const int STARTING_MANA = 4;
        const int MANA_GAIN_PER_TURN = 2;
        const int MAX_MANA_GAIN = 100;
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
        List<HP> HP;


        public Player ActivePlayer { get; set; } = Player.Player1;
        public List<Mana> Mana { get => mana; set => mana = value; }

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

        public Mana GetPlayerMana(Player player)
        {
            return Mana.Find(x => x.Player == player);
        }

        public Hand GetPlayerHand(Player player)
        {
            return hands.Find(x => x.Player == player);
        }

        public HP GetPlayerHP(Player player)
        {
            return HP.Find(x => x.Player == player);
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

        public bool CanPurchaseCard(Card card)
        {
            Mana currentPlayerMana = GetPlayerMana(card.Owner);
            foreach (var cost in card.TotalCosts)
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
            foreach (var cost in card.TotalCosts)
            {
                ManaType result = currentPlayerMana.ManaTypes.Find(x => x.Type == cost.Key);
                result.Amount -= cost.Value;
            }

        }
    }
}