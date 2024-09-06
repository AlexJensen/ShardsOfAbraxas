using UnityEngine;

namespace Abraxas.Popups
{
    class DialoguePopup : Popup
    {
        public override void SetActive(bool v)
        {
            gameObject.SetActive(v);
        }
    }
}
