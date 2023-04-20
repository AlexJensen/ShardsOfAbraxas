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

        int _startOfTurnManaAmount;
        #endregion

        #region Properties
        public int StartOfTurnManaAmount
        {
            get
            {
                return _startOfTurnManaAmount;
            }
            set
            {
                _startOfTurnManaAmount = value;
            }
        }
        #endregion

        #region Methods
        void Start()
        {
            StartOfTurnManaAmount = _settings.StartingMana;
        }

        public IEnumerator GenerateManaFromDeckRatio(Player player, int amount)
        {
            yield return GetPlayerMana(player).GenerateRatioMana(amount);
        }

        public Mana GetPlayerMana(Player player)
        {
            return _manas.Find(x => x.Player == player);
        }

        public bool CanPurchaseCard(Card card)
        {
            throw new NotImplementedException();
        }

        public void IncrementStartOfTurnManaAmount()
        {
            StartOfTurnManaAmount += _settings.ManaPerTurnIncrement;
        }
        #endregion
    }
}
