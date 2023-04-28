using Abraxas.Cards;
using System.Collections;

namespace Abraxas.Status
{
    class SummoningSicknessEffect : StatusEffect
    {
        public SummoningSicknessEffect(ICard card) : base(card)
        {
        }

        public override IEnumerator Combat()
        {
            yield return card.Combat();
        }
    }
}
