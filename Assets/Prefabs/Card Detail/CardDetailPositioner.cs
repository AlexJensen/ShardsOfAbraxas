using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailPositioner : Singleton<CardDetailPositioner>
{
    public enum ScreenSide
    {
        LEFT,
        RIGHT
    }

    public CardDetail cardDetail;
    RectTransform cardDetailRect;


    [Serializable]
    public struct Side
    {
        public ScreenSide side;
        public RectTransform rectTranform;
    }

    public List<Side> sides;

    protected override void Awake()
    {
        cardDetailRect = cardDetail.GetComponent<RectTransform>();
    }

    public void ShowCardDetailOnSide(Card card, ScreenSide side)
    {
        cardDetail.gameObject.SetActive(true);
        Side screenSide = sides.Find(x => x.side == side);
        cardDetailRect.SetParent(screenSide.rectTranform.transform);
        cardDetailRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, screenSide.rectTranform.rect.width);
        cardDetailRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, screenSide.rectTranform.rect.height);
    }

    public void HideCardDetail()
    {
        cardDetail.gameObject.SetActive(false);
    }
}
