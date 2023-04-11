using Abraxas.Behaviours.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Abraxas.Behaviours.Players;
using Zenject;

namespace Abraxas.Behaviours.Manas
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class ManaType : MonoBehaviour
    {
        #region Dependency Injections
        DataManager _dataManager;

        [Inject]
        public void Construct(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public class Factory : PlaceholderFactory<ManaType>
        {
        }
        #endregion

        #region Fields
        [SerializeField]
        TMP_Text _amountStr, _addStr;
        StoneData.StoneType _type;
        Player _player;
        Animator _animator;
        Image _image;
        int _amount = 0, _previousAmount = 0;
        #endregion

        #region Properties
        public StoneData.StoneType Type
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
        #endregion

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
                if (Player == Player.Player1) SetAnimationTrigger("AddManaDown");
                if (Player == Player.Player2) SetAnimationTrigger("AddManaUp");
                _previousAmount = _amount;
            }
        }

        public void Refresh()
        {
            StoneData.StoneDetails colorData = _dataManager.GetStoneDetails(Type);
            _image.color = colorData.color;
            name = colorData.name;
            _amountStr.text = _amount.ToString();
        }

        public void SetAnimationTrigger(string trigger)
        {
            _animator.SetTrigger(trigger);
        }
    }
}