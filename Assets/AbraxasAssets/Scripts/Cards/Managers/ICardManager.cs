using Abraxas.Cards.Controllers;

namespace Abraxas.Cards.Managers
{
    public interface ICardManager
    {
        void AddCard(ICardController card);
        ICardController GetCardFromIndex(int index);
        int GetCardIndex(ICardController card);
    }
}
