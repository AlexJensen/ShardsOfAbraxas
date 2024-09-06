using UnityEngine;

namespace Abraxas.Popups.Managers
{
    class PopupWindowManager : MonoBehaviour, IPopupWindowManager
    {
        #region Fields
        [SerializeField]
        private PopupWindowViewer _popupWindowViewer;
        #endregion

        #region Methods

        public IPopup PopupWindow<T>(bool val) where T : IPopup
        {
            return _popupWindowViewer.PopupWindow<T>(val);
        }
        #endregion
    }
}
