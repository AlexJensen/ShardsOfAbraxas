using System;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Models
{
    class  PlayerHealthModel : IPlayerHealthModel
    {
        #region Events
        public event Action OnHealthChanged;
        public event Action OnMaxHealthChanged;
        #endregion

        #region Dependencies
        Players.Player.Settings _playerSettings;
        [Inject]
        public void Construct(Players.Player.Settings playerSettings)
        {
            _playerSettings = playerSettings;
        }
        #endregion

        #region Fields
        int _HP, _maxHP;
        #endregion

        #region Properties
        public int HP
        {
            get => _HP;
            set
            {
                if (_HP == value) return;
                _HP = Math.Min(MaxHP, value);
                OnHealthChanged?.Invoke();
            }
        }
        public int MaxHP
        {
            get => _maxHP;
            set
            {
                if (_maxHP == value) return;
                _maxHP = value;
                OnMaxHealthChanged?.Invoke();
            }
        }

        public Player Player { get; set; }
        #endregion

        #region Methods
        public void Initialize()
        {
            MaxHP = _playerSettings.StartingHealth;
            HP = _playerSettings.StartingHealth;
        }
        #endregion
    }
}
