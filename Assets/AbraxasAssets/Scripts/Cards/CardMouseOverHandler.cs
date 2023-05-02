using Abraxas.CardViewers;
using Abraxas.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Abraxas.Cards
{
    [RequireComponent(typeof(Card))]
    public class CardMouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        Card _card;
        #endregion

        #region Properties
        public Card Card => _card = _card != null ? _card : GetComponent<Card>();
        #endregion

        #region Methods
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Card.Hidden) return;
            StartCoroutine(_cardViewerManager.ShowCardDetail(Card));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(_cardViewerManager.HideCardDetail());
        }
        #endregion
    }
}
