using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Events;
using Abraxas.StatusEffects.Types;
using System.Collections;

class PrefuelDecorator : CardDecorator
{
    public PrefuelDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator OnEventRaised(Event_CardChangedZones eventData)
    {
        yield return base.OnEventRaised(eventData);
        RequestRemoveStatusEffect<StatusEffect_SummoningSickness>();
        yield break;
    }
}
