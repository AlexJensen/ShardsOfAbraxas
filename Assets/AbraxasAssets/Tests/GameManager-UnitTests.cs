using System.Collections;
using Abraxas.Game;
using Abraxas.Zones.Decks;
using Abraxas.Zones.Hands;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Tests
{
    public class GameManagerUnitTests : ZenjectUnitTestFixture
    {
        [Inject] private readonly IGameManager _gameManager;
        [Inject] private readonly IDeckManager _deckManager;
        [Inject] private readonly IHandManager _handManager;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Load the scene that contains the manager classes
            SceneManager.LoadScene("Battlefield");

            // Install any necessary dependencies here
            Container.BindInterfacesAndSelfTo<GameManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<DeckManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<HandManager>().FromComponentInHierarchy().AsSingle();
            Container.Inject(this);
        }
    }
}
