using Abraxas.Events;
using Abraxas.StatusEffects;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;
using System.Drawing;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// ICardControllerInternal is an interface for non publically accessable Card Controller fields and methods. These should only be used by Card Controllers for internal decorator patterning processes.
    /// </summary>
    internal interface ICardControllerInternal : ICardController
    {
        ICardControllerInternal Aggregator { get; }

        void ApplyStatusEffect(IStatusEffect effect);
        bool HasStatusEffect<T>() where T : IStatusEffect;
        void RemoveStatusEffect<T>() where T : IStatusEffect;
        IEnumerator OnEventRaised(Event_ManaModified eventData);
        IEnumerator OnEventRaised(Event_GameStateEntered eventData);
        IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData);
        IEnumerator OnEventRaised(Event_CardChangedZones eventData);
        bool ShouldReceiveEvent(Event_ManaModified eventData);
        bool ShouldReceiveEvent(Event_CardChangedZones eventData);
        bool ShouldReceiveEvent(Event_GameStateEntered eventData);
        bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData);
        IEnumerator PreMovementAction(IFieldController field);
        IEnumerator PostMovementAction(IFieldController field);
        IEnumerator RangedAttack(IFieldController field, bool doAttack);
        ICardController CheckRangedAttack(IFieldController field, Point movement);

        #region Flags
        bool EnablePreMovementRangedAttack { get; set; }
        bool EnablePostMovementRangedAttack { get; set; }
        bool HasAttacked { get; set; }
        #endregion
    }
}
