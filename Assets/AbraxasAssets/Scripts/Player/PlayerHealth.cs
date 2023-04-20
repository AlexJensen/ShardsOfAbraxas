using UnityEngine;
using TMPro;
using Zenject;

namespace Abraxas.Players
{
    [RequireComponent(typeof(Animator))]
    public class PlayerHealth : MonoBehaviour
    {
        public enum HealthChangeAnimationTriggers
        {
            AddHPDown,
            AddHPUp,
            AddMaxHPDown,
            AddMaxHPUp,
        }

        #region Dependencies
        Player.Settings _playerSettings;
        [Inject]
        public void Construct(Player.Settings playerSettings)
        {
            _playerSettings = playerSettings;
        }

        #endregion

        #region Fields
        [SerializeField]
        TMP_Text _HPText, _addHPText, _addMaxHPText;

        [SerializeField]
        Players _player;

        Animator _animator;

        int _currentHP, _maxHP;
        int _previousHP, _previousMaxHP;
        #endregion

        #region Properties
        public Players Player { get => _player; set => _player = value; }

        public int CurrentHP
        {
            get => _currentHP; 
            set
            {
                if (_previousHP == _currentHP)
                {
                    _previousHP = _currentHP;
                }
                _currentHP = Mathf.Min(MaxHP,value);
                Refresh();
            }
        }
        public int MaxHP
        {
            get => _maxHP;
            set
            {
                if (_previousHP == _currentHP)
                {
                    _previousMaxHP = _maxHP;
                }
                _maxHP = value;
                Refresh();
            }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            MaxHP = _playerSettings.StartingHealth;
            CurrentHP = _playerSettings.StartingHealth;
            _previousMaxHP = MaxHP;
            _previousHP = CurrentHP;
        }

        private void LateUpdate()
        {
            UpdateHpText(ref _previousHP, _currentHP, _addHPText, HealthChangeAnimationTriggers.AddHPUp, HealthChangeAnimationTriggers.AddHPDown);
            UpdateHpText(ref _previousMaxHP, _maxHP, _addMaxHPText, HealthChangeAnimationTriggers.AddMaxHPUp, HealthChangeAnimationTriggers.AddMaxHPDown);
        }

        private void UpdateHpText(ref int previousValue, int currentValue, TMP_Text text, HealthChangeAnimationTriggers animationTriggerUp, HealthChangeAnimationTriggers animationTriggerDown)
        {
            if (previousValue != currentValue)
            {
                int change = currentValue - previousValue;
                text.text = $"{(change >= 0 ? "+" : "")}{change}";
                if (Player == Players.Player1) SetAnimationTrigger(change >= 0 ? animationTriggerUp : animationTriggerDown);
                if (Player == Players.Player2) SetAnimationTrigger(change >= 0 ? animationTriggerDown : animationTriggerUp);
                previousValue = currentValue;
            }
        }

        private void Refresh()
        {
            _HPText.text = $"{_currentHP + "/" + _maxHP}";
        }

        private void SetAnimationTrigger(HealthChangeAnimationTriggers trigger)
        {
            _animator.SetTrigger(trigger.ToString());
        }
        #endregion
    }
}
