using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Netcode;
using System;

/// <summary>
/// Represents a Card object, stores information about the current state of the card, and includes methods to move and scale the card.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(StatBlock))]
public class Card : NetworkBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Constants
    /// <summary>
    /// All locations the card can be in.
    /// </summary>
    public enum Zone
    {
        DECK, DRAG, HAND, PLAY, DEAD, BANISHED
    }

    public const float MOVE_FROM_HAND_TO_DRAG_SCALE_TIME = .2f;
    public const float MOVEMENT_ON_FIELD_TIME = .2f;
    public const float MOVE_TO_HAND_SCALE_TIME = .2f;
    public const float MOVE_TO_HAND_MOVE_TIME = .2f;
    public const float MOVE_FROM_HAND_TO_CELL_SCALE_TIME = 0.2f;
    public const float MOVE_FROM_HAND_TO_CELL_MOVE_TIME = 0.2f;
    #endregion

    #region Fields
    [SerializeField]
    readonly string title = "";

    Dictionary<StoneData.StoneType, int> totalCosts;
    string totalCostText;

    public Zone zone;

    [HideInInspector]
    public Vector2Int fieldPos;

    public Game.Player controller, owner;

    RectTransform rectTransform;
    [SerializeField]
    GameObject cover;

    [SerializeField]
    bool hidden = false;

    List<Stone> stones;

    StatBlock statBlock;

    [SerializeField]
    TMP_Text titleText, costText, statsText;

    [SerializeField]
    Image image;

    Canvas canvas;
    GraphicRaycaster graphicRaycaster;
    #endregion

    #region Properties
    public RectTransform RectTransform => rectTransform = rectTransform != null ? rectTransform : (RectTransform)transform;
    public List<Stone> Stones => stones ??= Enumerable.ToList(GetComponents<Stone>());
    public StatBlock StatBlock => statBlock = statBlock != null ? statBlock : GetComponent<StatBlock>();
    public string Title => title;
    public Image Image => image;
    public Canvas Canvas => canvas = canvas != null ? canvas : GetComponentInParent<Canvas>();
    public GraphicRaycaster GraphicRaycaster => graphicRaycaster = graphicRaycaster != null ? graphicRaycaster : Canvas.GetComponent<GraphicRaycaster>();

    public string TotalCostText { get => totalCostText; private set => totalCostText = value; }
    public Dictionary<StoneData.StoneType, int> TotalCosts { get => totalCosts ??= GenerateTotalCost(); }
    #endregion

    #region Unity Methods

    private Dictionary<StoneData.StoneType, int> GenerateTotalCost()
    {
        totalCosts = new Dictionary<StoneData.StoneType, int>();
        foreach (Stone stone in Stones)
        {
            stone.card = this;
            if (!TotalCosts.ContainsKey(stone.StoneType))
            {
                TotalCosts.Add(stone.StoneType, stone.Cost);
            }
            else
            {
                TotalCosts[stone.StoneType] += stone.Cost;
            }
        }
        return totalCosts;
    }

    void Update()
    {
        if (titleText.text != Title) titleText.text = Title;
        if (statsText.text != StatBlock.statsStr) statsText.text = StatBlock.statsStr;
        FormatCost();
        if (costText.text != TotalCostText) costText.text = TotalCostText;
        if (hidden != cover.activeInHierarchy) cover.SetActive(hidden);
    }

    private void FormatCost()
    {
        TotalCostText = "";
        foreach (KeyValuePair<StoneData.StoneType, int> pair in TotalCosts)
        {
            if (pair.Value != 0)
                TotalCostText += "<#" + ColorUtility.ToHtmlStringRGB(Game.Instance.stoneData.stoneColors.Find(x => x.type == pair.Key).color) + ">" + pair.Value;
        }
    }
    #endregion

    #region Input Events
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!hidden)
        {
            switch (zone)
            {
                case Zone.HAND:
                    {
                        if (controller == Game.Instance.CurrentPlayer)
                        {
                            transform.SetParent(Drag.Instance.transform);
                            Drag.Instance.card = this;
                            Hands.Instance.PlayerHands.Find(x => x.player == controller).RemoveCard(this);
                            StartCoroutine(ChangeScale(Drag.Instance.templateCardRect.rect.size, MOVE_FROM_HAND_TO_DRAG_SCALE_TIME));
                            zone = Zone.DRAG;
                        }
                    }
                    break;
            }
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
                    GraphicRaycaster.Raycast(eventData, results);

                    foreach (var hit in results)
                    {
                        Cell cell = hit.gameObject.GetComponent<Cell>();
                        if (cell)
                        {
                            if (cell.Cards.Count == 0 && cell.player == controller)
                            {
                                StopCoroutine(nameof(ChangeScale));
                                RequestMoveToCellServerRpc(cell.fieldPos);
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

    #region Animation Lerps
    public IEnumerator ChangeScale(Vector2 size, float time)
    {
        Vector2 origSize = RectTransform.rect.size;
        float lerpIncrement = 1 / time, lerpProgress = 0;
        while (lerpProgress <= 1)
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

    public IEnumerator MoveToHand(Hand hand)
    {
        hand.cardReturning = true;

        yield return StartCoroutine(Utilities.WaitForCoroutines(this,
            ChangeScale(hand.CardPlaceholder.CardPlaceholderRect.rect.size, MOVE_TO_HAND_SCALE_TIME),
            MoveTo(hand.CardPlaceholder.CardPlaceholderRect.position, MOVE_TO_HAND_MOVE_TIME)));

        hand.cardReturning = false;
        hand.AddCardAtPlaceholder(this);
    }

    public IEnumerator MoveToCell(Cell cell)
    {
        transform.SetParent(Drag.Instance.transform);
        Hands.Instance.PlayerHands.Find(x => x.player == controller).RemoveCard(this);
        yield return StartCoroutine(Utilities.WaitForCoroutines(this,
           ChangeScale(cell.RectTransform.rect.size, MOVE_FROM_HAND_TO_CELL_SCALE_TIME),
           MoveTo(cell.RectTransform.position, MOVE_FROM_HAND_TO_CELL_MOVE_TIME)));
        Field.Instance.AddToField(this, cell);
    }

    public IEnumerator OnCombat()
    {
        yield return StartCoroutine(Field.Instance.MoveCard(this, new Vector2Int(controller == Game.Player.Player1 ? statBlock[StatBlock.StatValues.MV] :
                                                                                 controller == Game.Player.Player2 ? -statBlock[StatBlock.StatValues.MV] : 0, 0)));
    }
    #endregion

    #region Server Methods
    [ServerRpc(RequireOwnership = false)]
    void RequestMoveToCellServerRpc(Vector2Int fieldPos)
    {
        if (!IsServer) return;
        MoveToCellClientRpc(fieldPos);
    }

    [ClientRpc]
    void MoveToCellClientRpc(Vector2Int fieldPos)
    {
        if (!IsClient) return;
        StartCoroutine(Field.Instance.MoveToFieldPosition(this, fieldPos));
    }

    #endregion
}


