﻿using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Zenject;

namespace Abraxas.StatusEffects.Types
{
    internal class StatusEffect_Flight : StatusEffect
    {
        public override void ApplyEffect(ICardController card)
        {
            
        }

        public override ICardController GetDecorator(ICardController card, ICardModel model, ICardView view, DiContainer container)
        {
            return container.Instantiate<FlightDecorator>(new object[] { card, model, view });
        }
    }
}