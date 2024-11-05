using Abraxas.Cards.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Abraxas.Cards.Views
{
    /// <summary>
    /// CardMouseOverlistener listens for mouse over events on a card and reports them to a CardMouseOverHandler.
    /// </summary>
    public class CardMouseOverListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Dependencies
        ICardMouseOverHandler _mouseOverHandler;
        internal void Initialize(ICardMouseOverHandler mouseOverListener)
        {
            _mouseOverHandler = mouseOverListener;
        }
        #endregion

        #region Methods
        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOverHandler.OnPointerEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOverHandler.OnPointerExit();
        }
        #endregion
    }
}
