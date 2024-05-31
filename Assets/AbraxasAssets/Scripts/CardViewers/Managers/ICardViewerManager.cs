using Abraxas.Cards.Controllers;

namespace Abraxas.CardViewers
{
    public interface ICardViewerManager
    {
        void HideCardViewer();
        void ShowCardViewer(ICardController card);
    }
}
