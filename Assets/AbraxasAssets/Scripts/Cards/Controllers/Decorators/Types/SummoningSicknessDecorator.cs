using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.StatusEffects.Types;
using System.Collections;

namespace Abraxas.Cards.Controllers
{
    class SummoningSicknessDecorator : CardControllerDecorator
    {
        public SummoningSicknessDecorator(ICardController innerController, ICardModel model, ICardView view)
            : base(innerController, model, view) { }


        public override IEnumerator Combat()
        {
            if (_innerController.HasStatusEffect<StatusEffect_SummoningSickness>())
            {
                yield return false;
            }

            yield return _innerController.Combat();
        }
    }
}
