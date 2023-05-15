using System;
using System.Collections.Generic;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Managers
{
    public class HealthManager : MonoBehaviour, IHealthManager
    {
        #region Fields
        [SerializeField]
        List<PlayerHealth> _hps;
        #endregion


        #region Methods
        public void ModifyPlayerHealth(Player player, int amount)
        {
            GetPlayerHealth(player).CurrentHP += amount;
        }

        private PlayerHealth GetPlayerHealth(Player player)
        {
            if (player == Player.Neutral)
            {
                throw new ArgumentException("Cannot get health for neutral player.");
            }

            PlayerHealth playerHP = _hps.Find(x => x.Player == player);
            if (playerHP == null)
            {
                throw new ArgumentException($"No health value found for player {player}.");
            }

            return playerHP;
        }
        #endregion
    }
}
