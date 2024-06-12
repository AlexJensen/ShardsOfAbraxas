using Abraxas.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Abraxas
{
    /// <summary>
    /// Button to toggle the display of cards within a zone
    /// </summary>
    public class ZoneButton : MonoBehaviour, IPointerClickHandler
    {
        #region Dependencies
        IUIManager _uiManager;
        [Inject]
        public void Construct(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }

        #endregion

        #region Fields
        [SerializeField] private GameObject zone;
        #endregion

        #region Methods
        public void OnPointerClick(PointerEventData eventData)
        {
            _uiManager.DisplayZone(zone);
        }
        #endregion
    }
}
