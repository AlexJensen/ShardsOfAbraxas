using Abraxas.Cards.Controllers;
using System.Collections.Generic;

namespace Abraxas.Cards.Managers
{
    public interface ICardManager
    {
        List<ICardController> Cards { get; }

        void AddCard(ICardController card);
        ICardController GetCardFromIndex(int index);
        int GetCardIndex(ICardController card);
    }
}
