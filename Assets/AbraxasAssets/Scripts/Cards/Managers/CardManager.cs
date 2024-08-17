using Abraxas.Cards.Controllers;
using System.Collections.Generic;

namespace Abraxas.Cards.Managers
{

    /// <summary>
    /// CardManager maintains a network synced index of all created cards during a game and is used primarily to identify cards between clients.
    /// </summary>
	class CardManager : ICardManager
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
