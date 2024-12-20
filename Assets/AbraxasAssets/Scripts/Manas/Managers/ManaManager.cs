using Abraxas.Cards.Controllers;
using Abraxas.Manas.Controllers;
using Abraxas.Manas.Models;
using Abraxas.Manas.Views;
using Abraxas.Network.Managers;
using Abraxas.Stones.Controllers;
using Abraxas.Zones.Decks.Controllers;
using System.Collections.Generic;
using Unity.Netcode;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas.Managers
{

    /// <summary>
    /// ManaManager is a class for managing player mana.
    /// </summary>

    class ManaManager : NetworkedManager, IManaManager
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
        public void GenerateManaFromDeckRatio(Player player, int amount)
        {
            if (IsClient)
            {
                // Send request to server
                GenerateManaFromDeckRatioServerRpc(player, amount);
            }
            else if (IsServer)
            {
                // Generate mana on the server
                var generatedManaAmounts = GetPlayerMana(player).GenerateRatioMana(amount);
                GetPlayerMana(player).ApplyGeneratedMana(generatedManaAmounts);
                // Send generated mana amounts to clients
                GenerateManaFromDeckRatioClientRpc(player, generatedManaAmounts);
            }
        }

        [ServerRpc]
        private void GenerateManaFromDeckRatioServerRpc(Player player, int amount)
        {
            var generatedManaAmounts = GetPlayerMana(player).GenerateRatioMana(amount);
            GetPlayerMana(player).ApplyGeneratedMana(generatedManaAmounts);
            // Send generated mana amounts to clients
            GenerateManaFromDeckRatioClientRpc(player, generatedManaAmounts);
        }

        [ClientRpc]
        private void GenerateManaFromDeckRatioClientRpc(Player player, ManaAmounts generatedManaAmounts)
        {
            GetPlayerMana(player).ApplyGeneratedMana(generatedManaAmounts);
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

        public void PurchaseStoneActivation(IStoneController stone)
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
            GetPlayerMana(deck.Player).GenerateRatioMana(0);
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
