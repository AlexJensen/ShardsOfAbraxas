using Abraxas.Cards.Controllers;
using System.Collections;

namespace Abraxas.CardViewers
{
    public interface ICardViewerManager
    {
        void HideCardViewer();
        void ShowCardViewer(ICardController card);
    }
}
