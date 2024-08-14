
using Abraxas.Manas.Controllers;
using Abraxas.Manas.Models;
using Abraxas.Players.Managers;
using System.Collections;
using TMPro;

using UnityEngine;
using Zenject;
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
        IManaController _controller;
        IPlayerManager _playerManager;

        [Inject]
        public void Construct(IPlayerManager playerManager)
        {
            _playerManager = playerManager;
        }
        public void Initialize(IManaModel model, IManaController controller)
        {
            _model = model;
            _controller = controller;
            _model.OnStartOfTurnManaChanged += RefreshVisuals;
            RefreshVisuals();
        }

        public void OnDestroy()
        {
            _controller.OnDestroy();
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
        public Player Player => _playerManager.LocalPlayer == Player.Player1 ? _player : (_player == Player.Player1 ? Player.Player2 : Player.Player1);
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