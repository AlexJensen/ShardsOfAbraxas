using Abraxas.Cards.Controllers;
using System.Collections;

namespace Abraxas.CardViewers
{
    public interface ICardViewerManager
    {
        IEnumerator HideCardViewer();
        IEnumerator ShowCardViewer(ICardController card);
    }
}
