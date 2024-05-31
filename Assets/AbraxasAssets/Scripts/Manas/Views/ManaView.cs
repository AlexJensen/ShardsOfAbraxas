
using TMPro;

using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas.Views
{

    /// <summary>
    /// ManaView is a MonoBehaviour that represents the visual component for a mana bar.
    /// </summary>

    class ManaView : MonoBehaviour, IManaView
    {

        #region Fields
        [SerializeField]
        Player _player;


        [SerializeField]
        TMP_Text _manaPerTurnText;


        #endregion

        #region Properties
        public Transform Transform { get => transform; }
        public Player Player { get => _player; }
        #endregion
    }
}