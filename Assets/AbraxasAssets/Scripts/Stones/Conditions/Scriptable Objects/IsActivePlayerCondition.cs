using Abraxas.Cards.Controllers;
using Abraxas.Players.Managers;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/IsActivePlayer")]
    public class IsActivePlayerCondition : Condition<object>
    {
        public enum PlayerType
        {
            Active,
            Inactive
        }

        [SerializeField]
        PlayerType _playerType;

        [Inject]
        protected readonly IPlayerManager _playerManager;

        public PlayerType Type { get => _playerType; set => _playerType = value; }

        public override bool IsMet(ICardController card, object eventData)
        {
            return Type == PlayerType.Active
                ? card.Owner == _playerManager.ActivePlayer
                : card.Owner != _playerManager.ActivePlayer;
        }
    }
}