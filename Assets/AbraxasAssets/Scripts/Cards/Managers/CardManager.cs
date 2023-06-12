using Abraxas.Cards.Controllers;
using System.Collections.Generic;
using Unity.Netcode;

namespace Abraxas.Cards.Managers
{
    internal class CardManager : NetworkBehaviour, ICardManager
    {
        #region Fields
        readonly List<ICardController> _cards = new();
        #endregion
        #region Methods
        public void AddCard(ICardController card)
        {
            _cards.Add(card);
        }

        public int GetCardIndex(ICardController card)
        {
            if (!_cards.Contains(card)) return -1;
            return _cards.IndexOf(card);
        }

        public ICardController GetCardFromIndex(int index)
        {
            return _cards[index];
        }
        #endregion
    }
}
