using Abraxas.Cards;
using System.Collections;

namespace Abraxas.CardViewers
{
    public interface ICardViewerManager
    {
        IEnumerator HideCardDetail();
        IEnumerator ShowCardDetail(Card card);
    }
}
