using Abraxas.Menus;
using Abraxas.UI;
using UnityEngine;
using Zenject;

namespace Abraxas
{
    public class ExitGameButton : MonoBehaviour
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
#if UNITY_STANDALONE
                Application.Quit();
#endif
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });
        }

        #endregion
    }
}
