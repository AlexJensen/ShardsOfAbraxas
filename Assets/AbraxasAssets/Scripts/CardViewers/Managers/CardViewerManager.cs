using Abraxas.Cards.Controllers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.CardViewers.Managers
{
    /// <summary>
    /// CardViewerManager handles displaying detailed card and stone information inside the CardViewer UI component.
    /// </summary>
    public class CardViewerManager : MonoBehaviour, ICardViewerManager
    {
        #region Constants
        public enum ScreenSide
        {
            LEFT,
            RIGHT
        }

        [Serializable]
        public struct Side
        {
            public ScreenSide side;
            public RectTransform rectTranform;
        }
        #endregion

        #region Fields
        [SerializeField]
        CardViewer _cardViewer;
        [SerializeField]
        List<Side> _sides;

        private float _screenWidth;

        RectTransform _cardDetailRect;
        #endregion

        #region Properties
        RectTransform CardDetailRect { get => _cardDetailRect != null ? _cardDetailRect : _cardDetailRect = (RectTransform)_cardViewer.transform; }
        #endregion

        #region Methods
        private void Awake()
        {
            _screenWidth = Screen.width * 0.5f;
        }

        private void ShowCardViewerOnSide(ICardController card, ScreenSide side)
        {
            _cardViewer.gameObject.SetActive(true);
            Side screenSide = _sides.Find(x => x.side == side);
            CardDetailRect.SetParent(screenSide.rectTranform.transform);
            CardDetailRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, screenSide.rectTranform.rect.width);
            CardDetailRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, screenSide.rectTranform.rect.height);
            _cardViewer.ShowCardDetails(card);
        }

        public void HideCardViewer()
        {
            _cardViewer.gameObject.SetActive(false);
        }

        public void ShowCardViewer(ICardController card)
        {
            if (Input.mousePosition.x > _screenWidth)
            {
                ShowCardViewerOnSide(card, ScreenSide.LEFT);
            }
            else
            {
                ShowCardViewerOnSide(card, ScreenSide.RIGHT);
            }
        }
        #endregion
    }
}