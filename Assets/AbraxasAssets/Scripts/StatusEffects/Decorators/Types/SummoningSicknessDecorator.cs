using Abraxas.Cards.Controllers;
using System.Collections;

namespace Abraxas.StatusEffects.Types
{
    class SummoningSicknessDecorator : CardControllerDecorator
    {
        public SummoningSicknessDecorator(ICardController innerController) : base(innerController) { }

        public override IEnumerator Combat()
        {
            if (_innerController.HasStatusEffect<StatusEffect_SummoningSickness>())
            {
                yield return false;
            }

            yield return true;
        }
    }
}
