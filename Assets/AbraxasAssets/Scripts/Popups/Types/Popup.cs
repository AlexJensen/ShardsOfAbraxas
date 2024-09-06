using UnityEngine;

namespace Abraxas.Popups
{
    internal abstract class Popup : MonoBehaviour, IPopup
    {
        public abstract void SetActive(bool v);
    }
}
