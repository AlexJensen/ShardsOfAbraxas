using Abraxas.CardViewers;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// CardMouseOverHandler is a controller-level class for handling card mouse over events.
    /// </summary>
    class CardMouseOverHandler : ICardMouseOverHandler
    {
        #region Dependencies
        ICardController _cardController;
        readonly ICardViewerManager _cardViewerManager;
        public CardMouseOverHandler(ICardViewerManager cardViewerManager)
        {
            _cardViewerManager = cardViewerManager;
        }

        internal void Initialize(ICardController cardController)
        {
            _cardController = cardController;
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
    }
}
