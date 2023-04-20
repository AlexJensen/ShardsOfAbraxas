using Abraxas.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.CardViewers
{
    public class CardViewerManager : MonoBehaviour, ICardViewerManager
    {
        #region Constants
        enum ScreenSide
        {
            LEFT,
            RIGHT
        }

        [Serializable]
        struct Side
        {
            public ScreenSide side;
            public RectTransform rectTranform;
        }
        #endregion

        #region Fields
        [SerializeField]
        CardViewer _cardDetail;
        [SerializeField]
        List<Side> _sides;

        RectTransform _cardDetailRect;
        #endregion

        #region Properties
        RectTransform CardDetailRect { get =>  _cardDetailRect != null ? _cardDetailRect : _cardDetailRect = (RectTransform)_cardDetail.transform; }
        #endregion

        #region Methods
        private void ShowCardViewerOnSide(Card card, ScreenSide side)
        {
            if (_cardDetail == null)
            {
                throw new ArgumentException("Card detail not found on Card Viewer.");
            }
            _cardDetail.gameObject.SetActive(true);
            Side screenSide = _sides.Find(x => x.side == side);
            CardDetailRect.SetParent(screenSide.rectTranform.transform);
            CardDetailRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, screenSide.rectTranform.rect.width);
            CardDetailRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, screenSide.rectTranform.rect.height);
            _cardDetail.ShowCardDetails(card);
        }

        public IEnumerator HideCardDetail()
        {
            _cardDetail.gameObject.SetActive(false);
            yield break;
        }

        public IEnumerator ShowCardDetail(Card card)
        {
            if (Input.mousePosition.x > Screen.width * .5f)
            {
                ShowCardViewerOnSide(card, ScreenSide.LEFT);
            }
            else
            {
                ShowCardViewerOnSide(card, ScreenSide.RIGHT);
            }
            yield break;
        }
        #endregion
    }
}