using Abraxas.Manas;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Models;
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
            foreach (var _ in from condition in Conditions
                              where !condition.IsMet()
                              select new { })
            {
                return false;
            }

            return true;
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
