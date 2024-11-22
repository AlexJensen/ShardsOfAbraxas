using Abraxas.Events;
using Abraxas.Manas;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.EventListeners;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Abraxas.Stones.Triggers
{
    public class TriggerStone : StoneController, IConditional
    {
        #region Dependencies
        IManaManager _manaManager;

        [Inject]
        public void Construct(IManaManager manaManager, DiContainer container)
        {
            _manaManager = manaManager;

            // Register event listeners dynamically
            var triggerStoneSO = Model.StoneSO as TriggerStoneSO;
            if (triggerStoneSO != null)
            {
                foreach (var eventListener in triggerStoneSO.EventListeners.OfType<EventListenerSO<IEvent>>())
                {
                    container.Inject(eventListener);
                    eventListener.InitializeListener();
                }
            }
        }
        #endregion

        #region Fields
        List<ICondition> _conditions = new();
        List<EffectStone> _effects = new();
        #endregion

        #region Properties
        public List<EffectStone> Effects { get => _effects; set => _effects = value; }
        public List<ICondition> Conditions { get => _conditions; set => _conditions = value; }
        #endregion

        #region Methods
        public bool CheckConditions()
        {
            return Conditions.All(condition => condition.IsMet());
        }

        public IEnumerator InvokeTrigger(params object[] vals)
        {
            if (!_manaManager.CanPurchaseStoneActivation(this)) yield break;
            _manaManager.PurchaseStoneActivation(this);
            foreach (EffectStone effect in Effects)
            {
                yield return effect.TriggerEffect(vals);
            }
        }
        #endregion
    }
}
