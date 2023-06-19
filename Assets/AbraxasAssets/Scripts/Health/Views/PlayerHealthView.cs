using Abraxas.Health.Controllers;
using Abraxas.Health.Models;
using TMPro;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Views
{
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
        Players.Player.Settings _playerSettings;
        IPlayerHealthModel _model;
        [Inject]
        public void Construct(Players.Player.Settings playerSettings)
        {
            _playerSettings = playerSettings;
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
        Player _player;

        Animator _animator;

        bool _healthChanged;
        int _previousHP, _previousMaxHP;
        #endregion

        #region Properties
        public Player Player { get => _player; set => _player = value; }
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
        }

        private void SetAnimationTrigger(HealthChangeAnimationTriggers trigger)
        {
            _animator.SetTrigger(trigger.ToString());
        }
        #endregion
    }
}
