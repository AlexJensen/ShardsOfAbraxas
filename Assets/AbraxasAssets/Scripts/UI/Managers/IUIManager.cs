using UnityEngine;

namespace Abraxas.UI
{
    public interface IUIManager
    {
        float CurrentOpacity { get; }

        void DisplayZone(GameObject zone);
    }
}
