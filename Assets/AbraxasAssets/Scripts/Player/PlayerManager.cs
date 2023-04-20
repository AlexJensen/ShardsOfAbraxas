using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Players
{
        public class PlayerManager : MonoBehaviour, IPlayerManager
        {
        #region Fields
        Players _activePlayer = Players.Player1;

        [SerializeField]
        List<HP> _hps;
        #endregion

        #region Properties
        public Players ActivePlayer => _activePlayer;

        public void ModifyPlayerHealth(Players player, int amount)
        {
            throw new System.NotImplementedException();
        }

        public void ToggleActivePlayer()
        {
            _activePlayer = _activePlayer == Players.Player1 ? Players.Player2 : Players.Player1;
        }
        #endregion
    }
}
