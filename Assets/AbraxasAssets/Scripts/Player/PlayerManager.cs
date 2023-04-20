using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Players
{
        public class PlayerManager : MonoBehaviour, IPlayerManager
        {
        #region Fields
        Players _activePlayer = Players.Player1;

        [SerializeField]
        List<PlayerHealth> _hps;
        #endregion

        #region Properties
        public Players ActivePlayer => _activePlayer;

        public void ModifyPlayerHealth(Players player, int amount)
        {
            GetPlayerHealth(player).CurrentHP += amount;
        }

        public void ToggleActivePlayer()
        {
            _activePlayer = _activePlayer == Players.Player1 ? Players.Player2 : Players.Player1;
        }

        public PlayerHealth GetPlayerHealth(Players player)
        {
            if (player == Players.Neutral)
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
