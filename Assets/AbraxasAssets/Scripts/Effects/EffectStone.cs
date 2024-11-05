using Abraxas.Manas;
using System.Collections;
using Zenject;

namespace Abraxas.Stones.Controllers
{
    public abstract class EffectStone : StoneController
    {
        #region Dependencies
        IManaManager _manaManager;
        [Inject]
        public void Construct(IManaManager manaManger)
        {
            _manaManager = manaManger;
        }
        #endregion

        #region Methods
        public virtual IEnumerator TriggerEffect(object[] vals)
        {
            if (!_manaManager.CanPurchaseStoneActivation(this)) yield break;
            _manaManager.PurchaseStoneActivation(this);
            yield return PerformEffect(vals);
        }

        protected abstract IEnumerator PerformEffect(object[] vals);
        #endregion
    }
}