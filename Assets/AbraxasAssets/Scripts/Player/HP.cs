using UnityEngine;
using TMPro;
using Abraxas.Behaviours.Game;

namespace Abraxas.Behaviours.Players
{
    public class HP : MonoBehaviour
    {
        [SerializeField]
        TMP_Text _HP, _addHP, _addHPMax;

        [SerializeField]
        Player _player;

        Animator _animator;

        [SerializeField]
        int _hPValue, _hPMaxValue;
        int _previousHPValue, _previousHPMaxValue;

        public Player Player { get => _player; set => _player = value; }

        public int HPValue
        {
            get => _hPValue; 
            set
            {
                if (_previousHPValue == _hPValue)
                {
                    _previousHPValue = _hPValue;
                }
                _hPValue = value;
                Refresh();
            }
        }
        public int HPMaxValue
        {
            get => _hPMaxValue;
            set
            {
                if (_previousHPValue == _hPValue)
                {
                    _previousHPMaxValue = _hPMaxValue;
                }
                _hPMaxValue = value;
                Refresh();
            }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _previousHPMaxValue = HPMaxValue;
            _previousHPValue = HPValue;
        }

        private void LateUpdate()
        {
            if (_previousHPValue != _hPValue)
            {
                int hPChange = _hPValue - _previousHPValue;
                _addHP.text = hPChange >= 0 ? "+" + hPChange.ToString() : hPChange.ToString();
                if (Player == Player.Player1) SetAnimationTrigger("AddHPDown");
                if (Player == Player.Player2) SetAnimationTrigger("AddHPUp");
                _previousHPValue = _hPValue;
            }

            if (_previousHPMaxValue != _hPMaxValue)
            {
                int hPMaxChange = _hPMaxValue - _previousHPMaxValue;
                _addHPMax.text = hPMaxChange >= 0 ? "+" + hPMaxChange.ToString() : hPMaxChange.ToString();
                if (Player == Player.Player1) SetAnimationTrigger("AddMaxHPDown");
                if (Player == Player.Player2) SetAnimationTrigger("AddMaxHPUp");
                _previousHPMaxValue = _hPMaxValue;
            }
        }

        public void Refresh()
        {
            _HP.text = _hPValue.ToString() + "/" + _hPMaxValue.ToString();
        }

        public void SetAnimationTrigger(string trigger)
        {
            _animator.SetTrigger(trigger);
        }
    }
}
