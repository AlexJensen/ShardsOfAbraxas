﻿using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.StatusEffects.Types;
using System.Collections;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// A card with Summoning Sickness does not move or attack during combat. Summoning Sickness automatically expires at the end of the turn.
    /// </summary>
    class SummoningSicknessDecorator : CardControllerDecorator,
        IGameEventListener<Event_CardChangedZones>
    {
        public SummoningSicknessDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
            : base(innerController, model, view) { }

        public override void InitializeListeners()
        {
            base.InitializeListeners();
            _eventManager.AddListener(this as IGameEventListener<Event_CardChangedZones>);
        }


        public override IEnumerator Combat()
        {
            if (RequestHasStatusEffect<StatusEffect_SummoningSickness>())
            {
                yield return false;
            }
            else
            {
                yield return InnerController.Combat();
            }
        }

        // Listen to the game state change event to remove summoning sickness
        public override IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            if (_gameStateManager.State is EndState)
            {
                GetBaseCard().RequestRemoveStatusEffect<StatusEffect_SummoningSickness>();
                _eventManager.RemoveListener(this as IGameEventListener<Event_GameStateEntered>);
            }

            yield return base.OnEventRaised(eventData);
            yield break;
        }

        public override bool ShouldReceiveEvent(Event_GameStateEntered eventData) => true;
    }
}