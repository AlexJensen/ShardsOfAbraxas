using Abraxas.Menus;
using Abraxas.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Abraxas
{
    public class ConcedeButton : MonoBehaviour
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
                SceneManager.LoadScene("Main");
            });
        }

        #endregion
    }
}
