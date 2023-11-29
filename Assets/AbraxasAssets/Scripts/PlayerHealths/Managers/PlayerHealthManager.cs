using Abraxas.Health.Controllers;
using Abraxas.Health.Views;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Managers
{
    public class PlayerHealthManager : MonoBehaviour, IPlayerHealthManager
    {
        #region Dependencies
        [Inject]
        void Construct(PlayerHealthController.Factory healthFactory)
        {
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
        public void ModifyPlayerHealth(Player player, int amount)
        {
            GetPlayerHealth(player).HP += amount;
        }
        public IPlayerHealthController GetPlayerHealth(Player player)
        {
            if (player == Player.Neutral)
            {
                throw new ArgumentException("Cannot get health for neutral player.");
            }

            IPlayerHealthController playerHP = _hps.Find(x => x.Player == player);
            if (playerHP == null)
            {
                throw new ArgumentException($"No health value found for player {player}.");
            }

            return playerHP;
        }
        public void AddPlayerHealth(IPlayerHealthController playerHealthController)
        {
            _hps.Add(playerHealthController);
        }
        #endregion
    }
}
