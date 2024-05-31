using Abraxas.Cards.Controllers;
using Abraxas.Manas.Controllers;
using Abraxas.Manas.Views;
using Abraxas.Stones.Controllers;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas.Managers
{

    /// <summary>
    /// ManaManager is a class for managing player mana.
    /// </summary>

	class ManaManager : MonoBehaviour, IManaManager
    {
        #region Dependencies
        Mana.Settings _settings;
        [Inject]
        public void Construct(ManaController.Factory manaFactory, Mana.Settings settings)
        {
            _settings = settings;
            foreach (var manaView in FindObjectsOfType<ManaView>())
            {

                var manaController = manaFactory.Create(manaView);
                manaController.StartOfTurnMana = _settings.StartingMana;
                _manas.Add(manaController);
            }
        }
        #endregion

        #region Fields
        readonly List<IManaController> _manas = new();
        #endregion

        #region Methods
        public IEnumerator GenerateManaFromDeckRatio(Player player, int amount)
        {
            yield return GetPlayerMana(player).GenerateRatioMana(amount);
        }

        public bool CanPurchaseCard(ICardController card)
        {

            return GetPlayerMana(card.OriginalOwner).CanPurchaseCard(card);

        }

        public void PurchaseCard(ICardController card)
        {
            GetPlayerMana(card.OriginalOwner).PurchaseCard(card);


        }

        public bool CanPurchaseStoneActivation(IStoneController stone)
        {
            return GetPlayerMana(stone.Card.OriginalOwner).CanPurchaseStone(stone);
        }

        public void PuchaseStoneActivation(IStoneController stone)
        {
            GetPlayerMana(stone.Card.OriginalOwner).PurchaseStone(stone);
        }

        public void IncrementStartOfTurnManaAmount(Player player)
        {
            GetPlayerMana(player).StartOfTurnMana += _settings.ManaPerTurnIncrement;
        }

        public void InitializeManaFromDeck(IDeckController deck)
        {
            GetPlayerMana(deck.Player).CreateManaTypesFromDeck(deck);
        }

        public void InitializeManaFromDecks(List<IDeckController> decks)
        {
            foreach (var deck in decks)
            {
                InitializeManaFromDeck(deck);
            }
        }

        public int GetStartOfTurnMana(Player player)
        {
            return GetPlayerMana(player).StartOfTurnMana;
        }

        private IManaController GetPlayerMana(Player player)
        {
            IManaController playerMana = _manas.Find(x => x.Player == player);
            return playerMana;
        }

      
        #endregion
    }
}
