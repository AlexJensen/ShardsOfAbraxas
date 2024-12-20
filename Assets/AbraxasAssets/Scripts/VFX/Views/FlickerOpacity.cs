using Abraxas.UI;
using Abraxas.VFX.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Abraxas.UI
{
    public class FlickerOpacity : MonoBehaviour
    {

        #region Dependencies
        IVFXManager _vfxManager;
        [Inject]
        public void Construct(IVFXManager uiManager)
        {
            _vfxManager = uiManager;
        }
        #endregion

        #region Fields
        public Image image;
        #endregion

        #region Methods
        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(Flicker());
        }

        /// <summary>
        /// Alternates the opacity of the image between minOpacity and maxOpacity at a speed of flickerSpeed.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Flicker()
        {
            while (true)
            {

                var color = image.color;
                color.a = _vfxManager.CurrentFlickerOpacity;
                image.color = color;
                yield return null;
            }
        }
        #endregion
    }
}
