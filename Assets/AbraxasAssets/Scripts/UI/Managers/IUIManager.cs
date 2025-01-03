using Abraxas.Core;
using Abraxas.Menus;
using System;
using UnityEngine;

namespace Abraxas.UI
{
    public interface IUIManager : IManager
    {
        IMenu PushMenu<T>() where T : IMenu;
        void SetFade(bool active);
        void DisplayZone(GameObject zone);
        void HideAllUI();
        void ShowConfirmation(Action onConfirmAction);
    }
}
