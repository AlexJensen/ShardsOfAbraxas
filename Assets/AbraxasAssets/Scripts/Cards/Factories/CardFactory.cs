using Abraxas.Cards.Data;
using Abraxas.Cards.Views;
using Zenject;

namespace Abraxas.Cards.Factories
{
    class CardFactory : IFactory<CardData, ICardView>
    {
        public ICardView Create(CardData data)
        {
            return null;
        }
    }
}
