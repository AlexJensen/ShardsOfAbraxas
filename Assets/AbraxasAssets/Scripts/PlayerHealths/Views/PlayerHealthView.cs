using Abraxas.Health.Models;
using Abraxas.Players.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Views
{
    /// <summary>
    /// PlayerHealthView is a MonoBehaviour that displays the player's health.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    class PlayerHealthView : MonoBehaviour, IPlayerHealthView
    {
        #region Enums
        public enum HealthChangeAnimationTriggers
        {
            AddHPDown,
            AddHPUp,
            AddMaxHPDown,
            AddMaxHPUp,
        }
        #endregion

        #region Dependencies
        IPlayerHealthModel _model;

        IPlayerManager _playerManager;

        [Inject]
        public void Construct(IPlayerManager playerManager)
        {
            _playerManager = playerManager;
        }
        public void Initialize(IPlayerHealthModel model)
        {
            _model = model;

            _model.OnHealthChanged += Refresh;
            _model.OnMaxHealthChanged += Refresh;
        }
        #endregion

        #region Fields
        [SerializeField]
        TMP_Text _HPText, _addHPText, _addMaxHPText;

        [SerializeField]
        Material hpBarMaterial;

        [SerializeField]
        Image hpBarImage;

        [SerializeField]
        Player _player;

        Animator _animator;

        bool _healthChanged;
        int _previousHP, _previousMaxHP;
        #endregion

        #region Properties
        public Player Player => _playerManager.LocalPlayer == Player.Player1 ? _player : (_player == Player.Player1 ? Player.Player2 : Player.Player1);
        #endregion

        #region Methods
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            if (_healthChanged)
            {
                UpdateHpText(ref _previousHP, _model.HP, _addHPText, HealthChangeAnimationTriggers.AddHPUp, HealthChangeAnimationTriggers.AddHPDown);
                UpdateHpText(ref _previousMaxHP, _model.MaxHP, _addMaxHPText, HealthChangeAnimationTriggers.AddMaxHPUp, HealthChangeAnimationTriggers.AddMaxHPDown);

                hpBarMaterial.SetFloat("_HP", _model.HP);
                hpBarMaterial.SetFloat("_MaxHP", _model.MaxHP);

                _healthChanged = false;
            }
        }

        private void UpdateHpText(ref int previousValue, int currentValue, TMP_Text text, HealthChangeAnimationTriggers animationTriggerUp, HealthChangeAnimationTriggers animationTriggerDown)
        {
            if (previousValue != currentValue)
            {
                int change = currentValue - previousValue;
                text.text = $"{(change >= 0 ? "+" : "")}{change}";
                if (Player == Player.Player1) SetAnimationTrigger(change >= 0 ? animationTriggerUp : animationTriggerDown);
                if (Player == Player.Player2) SetAnimationTrigger(change >= 0 ? animationTriggerDown : animationTriggerUp);
                previousValue = currentValue;
            }
        }

        private void Refresh()
        {
            if (_HPText != null)
                _HPText.text = $"{_model.HP + "/" + _model.MaxHP}";
            _healthChanged = true;
        }

        private void SetAnimationTrigger(HealthChangeAnimationTriggers trigger)
        {
            _animator.SetTrigger(trigger.ToString());
        }
        #endregion
    }
}
