using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas.Views
{
    class ManaView : MonoBehaviour, IManaView
    {

        #region Fields
        [SerializeField]
        Player _player;

        #endregion

        #region Properties
        public Transform Transform { get => transform; }
        public Player Player { get => _player; }
        #endregion
    }
}