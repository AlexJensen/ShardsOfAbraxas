using Abraxas.Cards.Controllers;
using Abraxas.CardViewers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Abraxas.Cards.Views
{
    [RequireComponent(typeof(ICardView))]
    public class CardMouseOverListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Dependencies
        ICardViewerManager _cardViewerManager;
        [Inject]
        public void Construct(ICardViewerManager cardViewerManager)
        {
            _cardViewerManager = cardViewerManager;
        }
        #endregion

        #region Fields
        ICardMouseOverHandler _mouseOverHandler;
        #endregion

        #region Properties
        public ICardMouseOverHandler MouseOverHandler => _mouseOverHandler ??= GetComponent<ICardView>().Controller.MouseOverHandler;
        #endregion

        #region Methods
        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOverHandler.OnPointerEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(_cardViewerManager.HideCardViewer());
        }
        #endregion
    }
}
