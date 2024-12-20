using Abraxas.Menus;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.UI.Managers
{
    class UIManager : MonoBehaviour, IUIManager
    {
        #region Fields
        [SerializeField] private List<GameObject> _uiZones;  // Used for card zones like Deck and Graveyard
        [SerializeField] private List<Menu> _menus;  // Used for popups like GameOverPopup and generic menu popups
        [SerializeField] private GameObject _fade;  // Used for fading the screen and preventing user input in the game while navigating menus

        private readonly Stack<Menu> menuStack = new();

        #endregion

        #region Methods

        // Display a specific zone and hide others
        public void DisplayZone(GameObject zone)
        {
            foreach (var z in _uiZones)
            {
                z.SetActive(z == zone && !z.activeSelf);
            }
        }

        // Display or hide the fade
        public void SetFade(bool active)
        {
            _fade.SetActive(active);
        }

        // Adds a menu to the stack and displays it
        public IMenu PushMenu<T>() where T : IMenu
        {
            Menu newMenu = _menus.Find(p => p is T);

            if (newMenu == null)
            {
                Debug.LogWarning($"Popup of type {typeof(T).Name} not found.");
                return null;
            }

            // Deactivate the current top popup before adding the new one
            if (menuStack.Count > 0)
            {
                menuStack.Peek().SetActive(false);
            }

            newMenu.SetActive(true);
            menuStack.Push(newMenu);
            return newMenu;
        }

        // Removes the top popup from the stack and displays the next one if it exists
        public void PopMenu()
        {
            if (menuStack.Count > 0)
            {
                Menu topPopup = menuStack.Pop();
                topPopup.SetActive(false);

                if (menuStack.Count > 0)
                {
                    menuStack.Peek().SetActive(true);
                }
            }
        }

        // Clear all popups
        public void ClearMenus()
        {
            while (menuStack.Count > 0)
            {
                Menu menu = menuStack.Pop();
                menu.SetActive(false);
            }
        }

        // General Hide All UI
        public void HideAllUI()
        {
            _fade.SetActive(false);
            foreach (var zone in _uiZones) zone.SetActive(false);
            foreach (var menu in _menus) menu.SetActive(false);
            ClearMenus(); 
        }

        public void ShowConfirmation(System.Action onConfirmAction)
        {
            var confirmationPopup = PushMenu<ConfirmationPopup>() as ConfirmationPopup;
            if (confirmationPopup != null)
            {
                confirmationPopup.OnConfirm = () => onConfirmAction?.Invoke();
            }
        }
        #endregion
    }
}
