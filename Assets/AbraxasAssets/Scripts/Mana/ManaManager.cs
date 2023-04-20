using Abraxas.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas
{
    public class ManaManager : MonoBehaviour, IManaManager
    {
        #region Dependencies
        Mana.Settings _settings;
        [Inject]
        public void Construct(Mana.Settings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Fields
        [SerializeField]
        List<Mana> _manas;

        int _startOfTurnMana;
        #endregion

        #region Properties
        public int StartOfTurnMana
        {
            get => _startOfTurnMana;
            set => _startOfTurnMana = value;
        }
        #endregion

        #region Methods
        void Start()
        {
            StartOfTurnMana = _settings.StartingMana;
        }

        public IEnumerator GenerateManaFromDeckRatio(Player player, int amount)
        {
            yield return GetPlayerMana(player).GenerateRatioMana(amount);
        }

        public bool CanPurchaseCard(Card card)
        {
            return GetPlayerMana(card.Owner).CanPurchaseCard(card);            
        }

        public void PurchaseCard(Card card)
        {
            GetPlayerMana(card.Owner).PurchaseCard(card);
        }

        public void IncrementStartOfTurnManaAmount()
        {
            StartOfTurnMana += _settings.ManaPerTurnIncrement;
        }

        private Mana GetPlayerMana(Player player)
        {
            if (player == Player.Neutral)
            {
                throw new ArgumentException("Cannot get mana for neutral player.");
            }

            Mana playerMana = _manas.Find(x => x.Player == player);
            if (playerMana == null)
            {
                throw new ArgumentException($"No mana value found for player {player}.");
            }

            return playerMana;
        }
        #endregion
    }
}
