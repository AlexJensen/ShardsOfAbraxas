using Abraxas.Cards.Controllers;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas.Managers
{
    class ManaManager : MonoBehaviour, IManaManager
    {
        #region Dependencies
        Mana.Settings _settings;
        [Inject]
        public void Construct(Mana.Settings settings)
        {
            _settings = settings;

            StartOfTurnMana = _settings.StartingMana;
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
        public IEnumerator GenerateManaFromDeckRatio(Player player, int amount)
        {
            yield return GetPlayerMana(player).GenerateRatioMana(amount);
        }

        public bool CanPurchaseCard(ICardController card)
        {
            return GetPlayerMana(card.OriginalOwner).CanPurchaseCard(card.Model);            
        }

        public void PurchaseCard(ICardController card)
        {
            GetPlayerMana(card.OriginalOwner).PurchaseCard(card.Model);
        }

        public void IncrementStartOfTurnManaAmount()
        {
            StartOfTurnMana += _settings.ManaPerTurnIncrement;
        }

        public void InitializeManaFromDeck(IDeckController deck)
        {
            GetPlayerMana(deck.Player).CreateManaTypesFromDeck();
        }

        private Mana GetPlayerMana(Player player)
        {
            Mana playerMana = _manas.Find(x => x.Player == player);
            return playerMana;
        }
        #endregion
    }
}
