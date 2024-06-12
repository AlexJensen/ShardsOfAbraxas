using Abraxas.Events;
using Abraxas.Players.Managers;
using System;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [Serializable]
    [CreateAssetMenu(menuName = "Abraxas/Conditions/IsActivePlayer")]
    public class IsActivePlayerCondition : Condition<ActivePlayerChangedEvent>
    {
        #region Dependencies
        [Inject]
        protected readonly IPlayerManager _playerManager;

        public override void Initialize(TriggerStone stone, ICondition condition)
        {
            Player = ((IsActivePlayerCondition)condition).Player;
            base.Initialize(stone, condition);
        }
        #endregion

        #region Enums
        public enum PlayerType
        {
            Active,
            Inactive
        }
        #endregion

        #region Fields
        [SerializeField]
        PlayerType _player;
        #endregion

        #region Properties
        public PlayerType Player { get => _player; set => _player = value; }
        #endregion

        #region Methods
        public override bool IsMet()
        {
            return Player == PlayerType.Active
                ? Stone.Card.Owner == _playerManager.ActivePlayer
                : Stone.Card.Owner != _playerManager.ActivePlayer;
        }
        #endregion
    }
}