using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abraxas.Core;
using Abraxas.Scripts.States;
using Abraxas.Behaviours.Zones.Hands;
using Abraxas.Behaviours.Manas;
using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Players;
using Zenject;

namespace Abraxas.Behaviours.Game
{
    public class GameManager : MonoBehaviour 
    {
        #region Constants
        const int STARTING_MANA = 4;
        const int MANA_GAIN_PER_TURN = 2;
        const int MAX_MANA_GAIN = 100;
        #endregion

        public event Action<GameState> GameStateChanged;


        GameState _state;
        [Inject]
        readonly GameStateFactory _stateFactory;

        int _manaGain = STARTING_MANA;

        [SerializeField]
        List<Hand> hands;
        [SerializeField]
        List<Mana> mana;
        [SerializeField]
        List<HP> HP;


        public Player ActivePlayer { get; set; } = Player.Player1;
        public List<Mana> Mana { get => mana; set => mana = value; }

            

        public void Start()
        {
            StartCoroutine(SwitchToState(GameStates.GameNotStarted));
        }

        private IEnumerator SwitchToState(GameStates state)
        {
            if (_state != null)
            {
                yield return StartCoroutine(_state.OnExitState());
            }
            _state = _stateFactory.CreateState(state);
            GameStateChanged?.Invoke(_state);
            yield return StartCoroutine(_state.OnEnterState());
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
            StartCoroutine(SwitchToState(_state.NextState()));
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
            _manaGain = Math.Min(MAX_MANA_GAIN, _manaGain + MANA_GAIN_PER_TURN);
            currentPlayerMana.GenerateRatioMana(_manaGain);
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
        internal void DamagePlayer(Player player, int amount)
        {
            GetPlayerHP(player).HPValue -= amount;
        }


    }
}