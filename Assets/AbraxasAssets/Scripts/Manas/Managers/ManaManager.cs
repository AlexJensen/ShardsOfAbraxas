using Abraxas.Cards.Controllers;
using Abraxas.Manas.Controllers;
using Abraxas.Manas.Views;
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
        void Construct(ManaController.Factory manaFactory, Mana.Settings settings)
        {
            _settings = settings;
            foreach (var manaView in FindObjectsOfType<ManaView>())
            {
                _manas.Add(manaFactory.Create(manaView));
            }
            StartOfTurnMana = _settings.StartingMana;
        }
        #endregion

        #region Fields
        readonly List<IManaController> _manas = new();

        int _startOfTurnMana;
        #endregion

        #region Properties
        public int StartOfTurnMana
        {
            get => _startOfTurnMana;
            private set => _startOfTurnMana = value;
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
            GetPlayerMana(deck.Player).CreateManaTypesFromDeck(deck);
        }

        private IManaController GetPlayerMana(Player player)
        {
            IManaController playerMana = _manas.Find(x => x.Player == player);
            return playerMana;
        }
        #endregion
    }
}
