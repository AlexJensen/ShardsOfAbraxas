using Abraxas.Behaviours.Events;
using UnityEngine;

namespace Abraxas.Behaviours.Status
{
    /// TODO: This is not the correct way to do a status effect, when the system for status effects is implemented this will need to be refactored to use that system.
    public class LoadingDebuff : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Instance.OnBeginningStateStarted += DeleteMe;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnBeginningStateStarted -= DeleteMe;
        }

        private void DeleteMe(object[] vals)
        {
            Destroy(this);
        }
    }
}