using Abraxas.UI.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Abraxas.Menus
{
    class ConfirmationPopup : Menu
    {
        #region Fields
        public delegate void ConfirmAction();
        public ConfirmAction OnConfirm;
        #endregion

        #region Methods

        public void OnYesClicked()
        {
            OnConfirm?.Invoke();
        }
        #endregion
    }
}
