using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Popups
{
    class PopupWindowViewer : MonoBehaviour
    {
        [SerializeField] 
        List<Popup> _popupWindows;

        public IPopup PopupWindow<T>(bool val) where T : IPopup
        {
            foreach (Popup offPopup in _popupWindows)
            {
                offPopup.SetActive(false);
            }
            if (val)
            {
                // Find the first popup of type T in the list
                Popup popup = _popupWindows.Find(p => p is T);

                if (popup != null)
                {
                    popup.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"Popup of type {typeof(T).Name} not found.");
                }
                return popup;
            }
            return null;
        }
    }
}
