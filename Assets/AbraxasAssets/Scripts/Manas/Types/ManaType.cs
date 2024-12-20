using Abraxas.Manas.Controllers;
using Abraxas.Players.Managers;
using Abraxas.Stones;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Manas
{
    /// <summary>
    /// ManaType is a MonoBehaviour that visually represents one of the mana types on the mana bar.
    /// </summary>
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
        IPlayerManager _playerManager;
        [Inject]
        public void Construct(IPlayerManager playerManager, Stone.Settings stoneSettings)
        {
            _playerManager = playerManager;
            _stoneSettings = stoneSettings;
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
        public IManaController Mana { get; internal set; }
        #endregion

        #region Methods
        public void Initialize()
        {
            _image = GetComponent<Image>();
            _animator = GetComponent<Animator>();

            Refresh();
        }

        private void LateUpdate()
        {
            if (_previousAmount != _amount)
            {
                int change = _amount - _previousAmount;
                _addStr.text = change >= 0 ? "+" + change.ToString() : change.ToString();
                if (Player == Player.Player1) SetAnimationTrigger(_playerManager.LocalPlayer == Player.Player1 ? ManaChangeAnimationTriggers.AddManaDown : ManaChangeAnimationTriggers.AddManaUp);
                if (Player == Player.Player2) SetAnimationTrigger(_playerManager.LocalPlayer == Player.Player1 ? ManaChangeAnimationTriggers.AddManaUp : ManaChangeAnimationTriggers.AddManaDown);
                _previousAmount = _amount;
            }
        }

        public void Refresh()
        {
            Stone.Settings.StoneTypeDetails colorData = _stoneSettings.GetStoneTypeDetails(Type);
            _image.color = colorData.color;
            name = colorData.name;
            _amountStr.text = _amount.ToString();  
        }

        public void SetAnimationTrigger(ManaChangeAnimationTriggers trigger)
        {
            _animator.SetTrigger(trigger.ToString());
        }
        #endregion
    }
}