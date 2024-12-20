using UnityEngine;

namespace Abraxas.Menus
{
    internal class Menu : MonoBehaviour, IMenu
    {
        public virtual void SetActive(bool v)
        {
            gameObject.SetActive(v);
        }
    }
}
