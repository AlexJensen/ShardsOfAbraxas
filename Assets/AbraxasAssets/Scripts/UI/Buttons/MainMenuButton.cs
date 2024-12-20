using Abraxas.Menus;
using Abraxas.UI;
using UnityEngine;
using Zenject;

namespace Abraxas
{
    public class MainMenuButton : MonoBehaviour
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
            _uiManager.HideAllUI();
            _uiManager.SetFade(true);
            _uiManager.PushMenu<MainMenu>();
        }

        #endregion
    }
}
