using Abraxas.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Abraxas
{
    public class FlickerOpacity : MonoBehaviour
    {

        #region Dependencies
        IUIManager _uiManager;
        [Inject]
        public void Construct(IUIManager uiManager)
        {
            _uiManager = uiManager;
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
                color.a = _uiManager.CurrentOpacity;
                image.color = color;
                yield return null;
            }
        }
        #endregion
    }
}
