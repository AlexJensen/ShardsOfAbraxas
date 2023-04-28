using Abraxas.Cards;
using System.Collections;

namespace Abraxas.Status
{    public abstract class StatusEffect : ICard
    {
        #region Fields
        protected ICard card;
        #endregion

        #region Methods
        public StatusEffect(ICard card)
        {
            this.card = card;
        }

        public virtual IEnumerator Combat()
        {
            yield return card.Combat();
        }
        #endregion
    }
}