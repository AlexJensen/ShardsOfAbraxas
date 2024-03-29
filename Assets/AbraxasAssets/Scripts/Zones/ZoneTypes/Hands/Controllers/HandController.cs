﻿using Abraxas.Cards.Controllers;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Hands.Views;
using Zenject;

namespace Abraxas.Zones.Hands.Controllers
{
    class HandController : ZoneController, IHandController
    {
        #region Dependencies
        readonly IHandManager _handManager;

        public HandController(IHandManager handManager)
        {
            _handManager = handManager;
        }

        #endregion

        #region Properties
        public int CardPlaceholderSiblingIndex => ((IHandView)View).CardPlaceholderSiblingIndex;
        #endregion

        #region Methods
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
