using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Players
{
        public class PlayerManager : NetworkBehaviour, IPlayerManager
        {
        #region Fields
        NetworkVariable<Players> _activePlayer = new NetworkVariable<Players>(Players.Player1);

        [SerializeField]
        List<PlayerHealth> _hps;

        #endregion

        #region Properties
        public Players ActivePlayer => _activePlayer.Value;
        #endregion

        #region Methods
        public void ModifyPlayerHealth(Players player, int amount)
        {
            GetPlayerHealth(player).CurrentHP += amount;
        }

        public IEnumerator ToggleActivePlayer()
        {
            if (!IsServer) yield break;
            _activePlayer.Value = _activePlayer.Value == Players.Player1 ? Players.Player2 : Players.Player1;
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
