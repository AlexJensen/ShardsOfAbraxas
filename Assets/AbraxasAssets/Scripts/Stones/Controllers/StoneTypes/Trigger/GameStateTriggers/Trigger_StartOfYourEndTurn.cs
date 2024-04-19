using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.Players.Managers;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;

namespace Abraxas.Stones.Controllers
{
    public class Trigger_StartOfYourEndTurn : Trigger_StartOfState
    {
        #region Dependencies
        readonly IPlayerManager _playerManager;
        public Trigger_StartOfYourEndTurn(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }
        #endregion
        #region Methods
        public override IEnumerator OnEventRaised(GameStateEnteredEvent eventData)
        {
            yield return InvokeTrigger();
        }
        public override bool ShouldReceiveEvent(GameStateEnteredEvent eventData)
        {
            return eventData.State is EndState && Card.Zone is FieldController && Card.Owner == _playerManager.ActivePlayer;
        }
        #endregion
    }
}
