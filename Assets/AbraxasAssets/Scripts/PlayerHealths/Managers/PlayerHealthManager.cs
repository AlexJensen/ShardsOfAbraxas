using Abraxas.Games.Managers;
using Abraxas.Health.Controllers;
using Abraxas.Health.Views;
using Abraxas.Popups;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Managers
{
    /// <summary>
    /// PlayerHealthManager is a manager for player health.
    /// </summary>

	class PlayerHealthManager : MonoBehaviour, IPlayerHealthManager
    {
        #region Dependencies
        IGameManager _gameManager;
        [Inject]
        public void Construct(IGameManager gameManager, PlayerHealthController.Factory healthFactory)
        {
            _gameManager = gameManager;

            foreach (var cellView in FindObjectsOfType<PlayerHealthView>())
            {
                _hps.Add(healthFactory.Create(cellView));
            }
        }
        #endregion

        #region Fields
        readonly List<IPlayerHealthController> _hps = new();
        #endregion

        #region Methods
        public IEnumerator ModifyPlayerHealth(Player player, int amount)
        {
            var playerHealth = GetPlayerHealth(player);
            yield return playerHealth.SetHealth(playerHealth.HP + amount);
            if (playerHealth.HP <= 0)
            {
                yield return _gameManager.EndGame(player);
            }
        }
        public IPlayerHealthController GetPlayerHealth(Player player)
        {
            if (player == Player.Neutral)
            {
                throw new ArgumentException("Cannot get health for neutral player.");
            }

            IPlayerHealthController playerHP = _hps.Find(x => x.Player == player) ?? throw new ArgumentException($"No health value found for player {player}.");

            return playerHP;
        }
        public void AddPlayerHealthController(IPlayerHealthController playerHealthController)
        {
            _hps.Add(playerHealthController);
        }
        #endregion
    }
}
