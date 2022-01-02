using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Zone
{
    DECK, DRAG, HAND, PLAY, DEAD, BANISHED
}

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(StatBlock))]
public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public const float SCALE_TIME = .2f;
    public const float MOVE_TIME = .2f;
    public const float RETURN_TO_HAND_SCALE_TIME = 0.2f;
    public const float RETURN_TO_HAND_MOVE_TIME = 0.2f;

    [SerializeField]
    string title = "", cost;

    public Zone zone;
    public Vector2Int fieldPos;
    public GameData.Players controller, owner;

    RectTransform rectTransform;
    StatBlock statBlock;

    Canvas canvas;
    GraphicRaycaster graphicRaycaster;

    Transform origParent;

    public RectTransform RectTransform { get => rectTransform; }

    #region Input Events
    public void OnBeginDrag(PointerEventData eventData)
    {
        switch (zone)
        {
            case Zone.HAND:
                {
                    origParent = transform.parent;
                    transform.SetParent(Drag.Instance.transform);
                    Drag.Instance.card = this;
                    StartCoroutine(ChangeScale(Drag.Instance.templateCardRect.rect.size, SCALE_TIME));
                    zone = Zone.DRAG;
                }
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (zone)
        {
            case Zone.DRAG:
                {
                    RectTransform.position = Input.mousePosition;
                }
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Drag.Instance.card = null;
        switch (zone)
        {
            case Zone.DRAG:
                {
                    List<RaycastResult> results = new();
                    graphicRaycaster.Raycast(eventData, results);

                    foreach (var hit in results)
                    {
                        Cell cell = hit.gameObject.GetComponent<Cell>();
                        if (cell)
                        {
                            if (cell.Cards.Count == 0 && cell.player == controller)
                            {
                                StopCoroutine(nameof(ChangeScale));
                                Field.Instance.AddToField(this, cell);
                                return;
                            }
                        }
                    }

                    zone = Zone.HAND;
                    StopCoroutine(nameof(ChangeScale));
                    StartCoroutine(MoveToHand(Hands.Instance.PlayerHands.Find(x => x.player == controller)));
                }
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.mousePosition.x > Screen.width / 2)
        {
            CardDetailPositioner.Instance.ShowCardDetailOnSide(this, CardDetailPositioner.ScreenSide.LEFT);
        }
        else
        {
            CardDetailPositioner.Instance.ShowCardDetailOnSide(this, CardDetailPositioner.ScreenSide.RIGHT);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardDetailPositioner.Instance.HideCardDetail();
    }
    #endregion

    #region Unity Methods

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        statBlock = GetComponent<StatBlock>();
    }

    void Update()
    {
        if (title != "") name = title;
    }
    #endregion

    #region Animation Lerps
    public IEnumerator ChangeScale(Vector2 size, float time)
    {
        Vector2 origSize = RectTransform.rect.size;
        float lerpIncrement = 1 / time, lerpProgress = 0;
        while(lerpProgress <= 1)
        {
            Vector2 lerpUpdate = Vector2.Lerp(origSize, size, lerpProgress);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpUpdate.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lerpUpdate.y);

            lerpProgress += lerpIncrement * Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator MoveTo(Vector2 location, float time)
    {
        Vector2 origLoc = RectTransform.position;
        float lerpIncrement = 1 / time, lerpProgress = 0;
        while (lerpProgress <= 1)
        {
            Vector2 lerpUpdate = Vector2.Lerp(origLoc, location, lerpProgress);
            RectTransform.position = lerpUpdate;

            lerpProgress += lerpIncrement * Time.deltaTime;
            yield return null;
        }
        yield return null;
    } 

    public IEnumerator OnCombat()
    {
        yield return StartCoroutine(Field.Instance.MoveCard(this, new Vector2Int(controller == GameData.Players.PLAYER_1 ? statBlock.stats.MV:
                                                                                 controller == GameData.Players.PLAYER_2 ? -statBlock.stats.MV: 0, 0)));
    }

    public IEnumerator MoveToHand(Hand hand)
    {
        hand.cardReturning = true;

        Coroutine scale = StartCoroutine(ChangeScale(hand.CardPlaceholder.CardPlaceholderRect.rect.size, RETURN_TO_HAND_SCALE_TIME));
        Coroutine move = StartCoroutine(MoveTo(hand.CardPlaceholder.CardPlaceholderRect.position + 
            new Vector3(hand.CardPlaceholder.CardPlaceholderRect.rect.size.x / 2, -(hand.CardPlaceholder.CardPlaceholderRect.rect.size.y / 2), 0), RETURN_TO_HAND_MOVE_TIME));

        yield return scale;
        yield return move;

        hand.cardReturning = false;
        hand.AddCardAtPlaceholder(this);
    }

    #endregion


}


