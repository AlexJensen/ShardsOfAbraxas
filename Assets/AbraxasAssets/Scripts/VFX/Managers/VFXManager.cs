using Abraxas.VFX.Controllers;
using UnityEngine;

namespace Abraxas.VFX.Managers
{
    internal class VFXManager : MonoBehaviour, IVFXManager
    {
        #region Fields
        readonly FlickerEffectController _flickerEffectController = new();
        #endregion

        #region Properties
        public float CurrentFlickerOpacity => _flickerEffectController.CurrentFlickerOpacity;
        #endregion

        #region Methods
        private void Update()
        {
            _flickerEffectController.Update(Time.deltaTime);
        }
        #endregion
    }
}
