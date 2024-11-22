using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Events;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;

class PrefuelDecorator : CardControllerDecorator
{
    public PrefuelDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator OnEventRaised(Event_CardChangedZones eventData)
    {
        // Do not apply summoning sickness
        if (LastManas != null)
        {
            UpdatePlayabilityAndCostText();
        }
        StatBlock.ShowSymbols = Zone is IFieldController;
        yield break;
    }
}
