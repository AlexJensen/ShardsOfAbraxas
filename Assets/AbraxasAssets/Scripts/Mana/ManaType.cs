using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Abraxas.Stones;

using Player = Abraxas.Players.Players;
using Abraxas.Events;

namespace Abraxas.Manas
{
    [RequireComponent(typeof(Image))]
    public class ManaType : MonoBehaviour
    {
        #region Enums
        public enum ManaChangeAnimationTriggers
        {
            AddManaDown,
            AddManaUp,
        }
        #endregion

        #region Dependencies
        Stone.Settings _stoneSettings;
        EventManager _eventManager;
        [Inject]
        public void Construct(Stone.Settings stoneSettings, EventManager eventManager)
        {
            _stoneSettings = stoneSettings;
            _eventManager = eventManager;
        }

        public class Factory : PlaceholderFactory<ManaType>
        {
        }
        #endregion

        #region Fields
        [SerializeField]
        TMP_Text _amountStr, _addStr;
        StoneType _type;
        Player _player;
        Animator _animator;
        Image _image;
        int _amount = 0, _previousAmount = 0;
        #endregion

        #region Properties
        public StoneType Type
        {
            get => _type;
            set
            {
                _type = value;
                Refresh();
            }
        }

        public int Amount
        {
            get => _amount;
            set
            {
                if (_previousAmount == _amount)
                {
                    _previousAmount = _amount;
                }
                _amount = value;
                Refresh();
            }
        }

        public Player Player { get => _player; set => _player = value; }
        public Mana Mana { get; internal set; }
        #endregion

        #region Methods
        private void Awake()
        {
            _image = GetComponent<Image>();
            _animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            if (_previousAmount != _amount)
            {
                int change = _amount - _previousAmount;
                _addStr.text = change >= 0 ? "+" + change.ToString() : change.ToString();
                if (Player == Player.Player1) SetAnimationTrigger(ManaChangeAnimationTriggers.AddManaDown);
                if (Player == Player.Player2) SetAnimationTrigger(ManaChangeAnimationTriggers.AddManaUp);
                _previousAmount = _amount;
            }
        }

        public void Refresh()
        {
            Stone.Settings.StoneDetails colorData = _stoneSettings.GetStoneDetails(Type);
            _image.color = colorData.color;
            name = colorData.name;
            _amountStr.text = _amount.ToString();
            StartCoroutine(_eventManager.RaiseEvent(typeof(ManaModifiedEvent), new ManaModifiedEvent(Mana)));
        }

        public void SetAnimationTrigger(ManaChangeAnimationTriggers trigger)
        {
            _animator.SetTrigger(trigger.ToString());
        }
        #endregion
    }
}