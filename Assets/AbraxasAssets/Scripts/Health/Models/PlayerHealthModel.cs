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
        Player _player;
        #endregion

        #region Properties
        public int HP
        {
            get => _HP;
            set
            {
                if (_HP != value) OnHealthChanged.Invoke();
                _HP = Math.Min(MaxHP, value);
            }
        }
        public int MaxHP
        {
            get => _maxHP;
            set
            {
                if (_maxHP != value) OnMaxHealthChanged.Invoke();
                _maxHP = value;
            }
        }

        public Player Player
        {
            get => _player;
            set => _player = value;
        }
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
