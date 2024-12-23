using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.StatusEffects.Types;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;
using Zenject;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// A card with Summoning Sickness does not move or attack during combat. Summoning Sickness automatically expires at the end of the turn.
    /// </summary>
    class SummoningSicknessDecorator : CardDecorator
    {
        #region Dependencies
        IGameStateManager _gameStateManager;
        public SummoningSicknessDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
            : base(innerController, model, view) { }

        [Inject]
        public void Construct(IGameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        #endregion
        #region Methods

        public override IEnumerator Combat(IFieldController field)
        {
            if (RequestHasStatusEffect<StatusEffect_SummoningSickness>())
            {
                yield return false;
            }
            else
            {
                yield return InnerController.Combat(field);
            }
        }
        #endregion

        #region Event Listeners
        // Listen to the game state change event to remove summoning sickness
        public override IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            if (_gameStateManager.State is EndState)
            {
                RequestRemoveStatusEffect<StatusEffect_SummoningSickness>();
            }

            yield return base.OnEventRaised(eventData);
            yield break;
        }

        public override bool ShouldReceiveEvent(Event_GameStateEntered eventData) => true;
        #endregion
    }
}
