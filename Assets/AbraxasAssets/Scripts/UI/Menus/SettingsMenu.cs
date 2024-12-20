using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Menus
{
    class SettingsMenu : Menu
    {
        #region Fields
        [SerializeField] List<GameObject> windows;
        #endregion

        #region Methods
        public void ShowWindow(GameObject window)
        {
            foreach (var otherwindows in windows)
            {
                otherwindows.SetActive(false);
            }
            window.SetActive(true);
        }

        #endregion
    }
}
