using Abraxas.Manas;
using Abraxas.Stones.Controllers;

using Abraxas.Stones.Controllers.StoneTypes.Conditions;
using Abraxas.Stones.Models;
using System.Collections;
using System.Collections.Generic;
using Zenject;

namespace Abraxas.Stones
{

    public class TriggerStone : StoneController
    {
        #region Dependencies
        IManaManager _manaManager;
        [Inject]
        public void Construct(IManaManager manaManger)
        {
            _manaManager = manaManger;
        }
        public override void Initialize(IStoneModel model)
        {
            base.Initialize(model);
        }
        #endregion

        #region Fields
        List<EffectStone> _effects = new();
        List<ICondition> _conditions = new();
        #endregion

        #region Properties
        public List<EffectStone> Effects { get => _effects; set => _effects = value; }
        public List<ICondition> Conditions { get => _conditions; set => _conditions = value; }
        #endregion

        #region Methods
        public IEnumerator CheckConditions()
        {
            foreach (var condition in Conditions)
            {
                if (!condition.IsMet())
                {
                    yield break;
                }
            }
            yield return InvokeTrigger();
        }

        public IEnumerator InvokeTrigger(params object[] vals)
        {
            if (!_manaManager.CanPurchaseStoneActivation(this)) yield break;
            _manaManager.PuchaseStoneActivation(this);
            foreach (EffectStone effect in _effects)
            {
                yield return effect.TriggerEffect(vals);
            }
        }
        #endregion
    }
}
