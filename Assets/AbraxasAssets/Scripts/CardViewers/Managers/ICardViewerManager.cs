using Abraxas.Cards.Controllers;
using Abraxas.Core;

namespace Abraxas.CardViewers
{
    public interface ICardViewerManager: IManager
    {
        void HideCardViewer();
        void ShowCardViewer(ICardController card);
    }
}
