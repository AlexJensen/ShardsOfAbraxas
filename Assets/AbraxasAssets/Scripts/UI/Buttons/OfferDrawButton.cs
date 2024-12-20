using Abraxas.Menus;
using Abraxas.UI;
using UnityEngine;
using Zenject;

namespace Abraxas
{
    public class OfferDrawButton : MonoBehaviour
    {
        #region Dependencies
        private IUIManager _uiManager;
        [Inject]
        public void Construct(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        #endregion

        #region Methods
        public void OnClick()
        {
            _uiManager.ShowConfirmation(() =>
            {
                Debug.Log("Draw offered!");
                // Implement the draw logic here
            });
        }

        #endregion
    }
}
