
using Abraxas.Manas.Models;
using System.Collections;
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
        #region Dependencies
        IManaModel _model;
        public void Initialize(IManaModel model)
        {
            _model = model;
            _model.OnStartOfTurnManaChanged += RefreshVisuals;
            RefreshVisuals();
        }

        public void OnDestroy()
        {
            _model.OnStartOfTurnManaChanged -= RefreshVisuals;
        }
        #endregion



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

        #region Methods
        public void StartManaEventCoroutine(IEnumerator eventInvoke)
        {
            StartCoroutine(eventInvoke);
        }
        private void RefreshVisuals()
        {
            _manaPerTurnText.text = _model.StartOfTurnMana >= 0? "+" + _model.StartOfTurnMana.ToString(): _model.StartOfTurnMana.ToString();
        }
        #endregion
    }
}