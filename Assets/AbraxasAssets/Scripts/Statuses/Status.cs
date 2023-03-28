using Abraxas.Behaviours.Cards;
using System.Collections.Generic;

namespace Abraxas.Behaviours.Status
{    public abstract class Status
    {
        public List<Card> cardsAffected;
        public abstract void Set(params object[] vals);
        public abstract void Clear(params object[] vals);
    }
}