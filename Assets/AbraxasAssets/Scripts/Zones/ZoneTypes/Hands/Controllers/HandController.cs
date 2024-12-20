using Abraxas.Cards.Controllers;
using Abraxas.Players.Managers;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Decks.Controllers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Hands.Views;
using System.Collections;
using Unity.Netcode;

namespace Abraxas.Zones.Hands.Controllers
{

    class HandController : ZoneController, IHandController
    {
        #region Dependencies
        readonly IPlayerManager _playerManager;
        readonly IHandManager _handManager;

        public HandController(IPlayerManager playerManager, IHandManager handManager)
        {
            _handManager = handManager;
            _playerManager = playerManager;
        }

        #endregion

        #region Properties
        public int CardPlaceholderSiblingIndex => ((IHandView)View).CardPlaceholderSiblingIndex;
		#endregion


        #region Methods
        public override IEnumerator MoveCardToZone(ICardController card, int index = 0)
        {
            if (!NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost) card.Hidden = card.Owner != _playerManager.LocalPlayer && card.PreviousZone is DeckController;
            yield return base.MoveCardToZone(card, index);
        }

        public void OnUpdate()
        {
            if (View != null)
            {
                if (_handManager.CardDragging != null && _handManager.CardDragging.Owner == Player)
                {
                    ((IHandView)View).UpdateCardPlaceholderPosition();
                    return;
                }
            ((IHandView)View).HidePlaceholder();
            }
        }
        #endregion
    }
}
