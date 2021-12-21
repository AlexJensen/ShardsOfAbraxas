using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public LayoutElement cardPlaceholderLayout;
    public RectTransform cardPlaceholderRect;
    public GameData.Players player;
    List<Card> cards;
    float origHeight;
    public int currentIndex = 0, newIndex;
    bool expanding = false, retracting = false;   

    RectTransform rectTransform;

    const float PLACEHOLDER_SCALE_TIME = .2f;

    public RectTransform RectTransform { get => rectTransform; }

    private void Start()
    {
        origHeight = cardPlaceholderLayout.preferredHeight;
        rectTransform = GetComponent<RectTransform>();
        cards = GetComponentsInChildren<Card>().ToList();
        ResetPlaceholder();
    }

    private void ResetPlaceholder()
    {
        cardPlaceholderLayout.preferredHeight = 0;
        cardPlaceholderLayout.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Drag.Instance.card != null && Drag.Instance.card.controller == player)
        {
            cards.Remove(Drag.Instance.card);
            newIndex = currentIndex;
            cardPlaceholderLayout.gameObject.SetActive(true);
            foreach (Card card in cards)
            {
                if (player == GameData.Players.PLAYER_1?
                    Input.mousePosition.y > card.transform.position.y:
                    Input.mousePosition.y < card.transform.position.y)
                {
                    newIndex = cards.IndexOf(card);
                    break;
                }
                if (cards.IndexOf(card) == cards.Count - 1)
                {
                    newIndex = cards.IndexOf(card) + 1;
                    break;
                }
            }
            if (newIndex != currentIndex)
            {
                if (!retracting)
                {
                    if (cardPlaceholderLayout.preferredHeight == 0)
                    {
                        expanding = true;
                        cardPlaceholderLayout.transform.SetSiblingIndex(newIndex);
                        currentIndex = newIndex;
                        StartCoroutine(ScalePlaceholder(origHeight, PLACEHOLDER_SCALE_TIME));
                    }
                    else if (!expanding)
                    {
                        retracting = true;
                        StartCoroutine(ScalePlaceholder(0, PLACEHOLDER_SCALE_TIME));
                    }
                }
            }          
        }
        else if (cardPlaceholderLayout.preferredHeight > 0)
        {
            if (!expanding && !retracting)
            {
                retracting = true;
                StartCoroutine(ScalePlaceholder(0, PLACEHOLDER_SCALE_TIME));
            }
        }
        else
        {
            ResetPlaceholder();
        }
    }

    IEnumerator ScalePlaceholder(float height, float time)
    {
        float origHeight = cardPlaceholderLayout.preferredHeight;
        float lerpIncrement = 1 / time;
        float lerpProgress = 0;
        while (lerpProgress < 1)
        {
            lerpProgress += lerpIncrement * Time.deltaTime;
            lerpProgress = Mathf.Min(lerpProgress, 1);
            float lerpUpdate = Mathf.Lerp(origHeight, height, lerpProgress);
            cardPlaceholderLayout.preferredHeight = lerpUpdate;
            yield return null;
        }
        expanding = false;
        retracting = false;
    }
}
