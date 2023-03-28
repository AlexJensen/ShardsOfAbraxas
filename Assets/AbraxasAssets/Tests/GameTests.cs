/*using Abraxas.Components;
using Abraxas.Scripts;
using Abraxas.Data;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Abraxas.Tests
{
    public class GameTests
    {
        [UnityTest]
        public IEnumerator GameNotStartedState_CorrectlyInitializesGame()
        {
            // Arrange
            var game = new GameObject().AddComponent<Game>();
            var expectedStateType = typeof(GameNotStartedState);
            var expectedActivePlayer = Game.Player.Player1;

            // Act
            yield return new WaitForSeconds(0.1f); // wait for game initialization
            var currentStateType = game.currentState.GetType();
            var activePlayer = game.ActivePlayer;

            // Assert
            Assert.AreEqual(expectedStateType, currentStateType, "GameNotStartedState should be the initial state.");
            Assert.AreEqual(expectedActivePlayer, activePlayer, "The active player should be Player1 at the beginning of the game.");
        }

        [Test]
        public void GetPlayerMana_ReturnsCorrectMana()
        {
            // Arrange
            GameObject gameObject = new GameObject();
            Mana mana = gameObject.AddComponent<Mana>();

            // Act
            Game game = new Game();
            game.Mana = new List<Mana> { mana };

            // Assert
            Assert.AreEqual(game.GetPlayerMana(Game.Player.Player1), mana);
            Assert.AreEqual(game.GetPlayerMana(Game.Player.Player2), null);
        }

        [Test]
        public void SwitchActivePlayer_SwitchesToOtherPlayer()
        {
            // Arrange
            var game = new Game();
            game.ActivePlayer = Game.Player.Player1;

            // Act
            game.SwitchActivePlayer();

            // Assert
            Assert.AreEqual(Game.Player.Player2, game.ActivePlayer, "SwitchActivePlayer should switch the active player to the other player.");
        }

        [Test]
        public void GenerateManaForActivePlayer_GeneratesCorrectAmountOfMana()
        {
            // Arrange
            Game game = new GameObject().AddComponent<Game>(); // create a new Game object and add the Game component to it
            game.ActivePlayer = Game.Player.Player1;
            Mana playerMana = game.GetPlayerMana(Game.Player.Player1);
            playerMana.gameObject.AddComponent<ManaType>(); // add the ManaType component to the player's Mana game object
            int expectedManaAmount = 2;

            // Act
            game.GenerateManaForActivePlayer();

            // Assert
            Assert.AreEqual(expectedManaAmount, playerMana.gameObject.GetComponent<ManaType>().Amount, "Generated mana amount is incorrect");
        }

        [Test]
        public void CanPurchaseCard_ReturnsTrueIfPlayerHasEnoughMana()
        {
            // Arrange
            Game game = new GameObject().AddComponent<Game>();

            // Create player mana
            ManaType garnetMana = new GameObject().AddComponent<ManaType>();
            garnetMana.Type = StoneData.StoneType.GARNET;
            garnetMana.Amount = 3;

            ManaType amethystMana = new GameObject().AddComponent<ManaType>();
            amethystMana.Type = StoneData.StoneType.AMETHYST;
            amethystMana.Amount = 2;

            ManaType aquamarineMana = new GameObject().AddComponent<ManaType>();
            aquamarineMana.Type = StoneData.StoneType.AQUAMARINE;
            aquamarineMana.Amount = 1;

            ManaType[] manaTypes = { garnetMana, amethystMana, aquamarineMana };
            Mana playerMana = new GameObject().AddComponent<Mana>();
            playerMana.Player = Game.Player.Player1;
            playerMana.ManaTypes = manaTypes;

            // Add player mana to game
            game.Mana = new List<Mana> { playerMana };

            // Create a card with a cost that can be purchased with player mana
            Card card = new Card();
            card.Owner = Game.Player.Player1;
            card.TotalCosts = new Dictionary<StoneData.StoneType, int> {
        { StoneData.StoneType.GARNET, 2 },
        { StoneData.StoneType.AMETHYST, 1 }
    };

            // Act
            bool canPurchase = game.CanPurchaseCard(card);

            // Assert
            Assert.IsTrue(canPurchase);
        }
    }
}*/