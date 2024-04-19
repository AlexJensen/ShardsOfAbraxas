using Abraxas.Manas;
using Abraxas.Stones.Controllers;
using System.Collections;
using System.Collections.Generic;
using Zenject;

namespace Abraxas.Stones
{
    public abstract class TriggerStone : StoneController
    {
        #region Dependencies
        IManaManager _manaManager;
        [Inject]
        public void Construct(IManaManager manaManger)
        {
            _manaManager = manaManger;
        }
        #endregion

        #region Fields
        List<EffectStone> _effects = new();
        #endregion

        #region Properties
        public List<EffectStone> Effects { get => _effects; set => _effects = value; }
        #endregion

        #region Methods
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