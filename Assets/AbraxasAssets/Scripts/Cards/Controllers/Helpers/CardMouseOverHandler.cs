using Abraxas.Cards.Views;
using Abraxas.CardViewers;
using System;

namespace Abraxas.Cards.Controllers
{
    class CardMouseOverHandler : ICardMouseOverHandler
    {
        #region Dependencies
        ICardController _cardController;
        readonly ICardViewerManager _cardViewerManager;
        public CardMouseOverHandler(ICardController cardController, ICardViewerManager cardViewerManager)
        {
            _cardController = cardController;
            _cardViewerManager = cardViewerManager;
        }
        #endregion

        public void OnPointerEnter()
        {
            if (_cardController.Hidden) return;
            _cardViewerManager.ShowCardViewer(_cardController);
        }
        public void OnPointerExit()
        {
            _cardViewerManager.HideCardViewer();
        }

        internal void Initialize(ICardController cardController)
        {
            _cardController = cardController;
        }
    }
}
