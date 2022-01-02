using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(RectTransform))]
public class CardPlaceholder : MonoBehaviour
{
    LayoutElement cardPlaceholderLayout;
    RectTransform cardPlaceholderRect;
    public LayoutElement CardPlaceholderLayout { get => cardPlaceholderLayout; }
    public RectTransform CardPlaceholderRect { get => cardPlaceholderRect; }

    int currentIndex = 0, newIndex = 0;
    bool expanding = false, retracting = false;
    float origHeight;

    const float PLACEHOLDER_SCALE_TIME = .2f;

    private void Start()
    {
        cardPlaceholderLayout = GetComponent<LayoutElement>();
        cardPlaceholderRect = GetComponent<RectTransform>();

        origHeight = CardPlaceholderLayout.preferredHeight;
    }

    internal void Reset()
    {
        CardPlaceholderLayout.preferredHeight = 0;
        retracting = false;
        expanding = false;
        StopCoroutine(nameof(ScalePlaceholder));
        gameObject.SetActive(false);
    }

    internal void Initialize()
    {
        gameObject.SetActive(true);
    }

    internal void UpdateIndex(int index)
    {
        newIndex = index;
    }

    internal void CheckPosition()
    {
        if (newIndex != currentIndex)
        {
            if (!retracting)
            {
                if (CardPlaceholderLayout.preferredHeight == 0)
                {
                    expanding = true;
                    CardPlaceholderLayout.transform.SetSiblingIndex(newIndex);
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

    internal void Hide()
    {
        if (CardPlaceholderLayout.preferredHeight == 0)
        {
            Reset();
        }
        else if (!expanding && !retracting)
        {
            retracting = true;
            StartCoroutine(ScalePlaceholder(0, PLACEHOLDER_SCALE_TIME));
        }
    }

    IEnumerator ScalePlaceholder(float height, float time)
    {
        float origHeight = CardPlaceholderLayout.preferredHeight;
        float lerpIncrement = 1 / time;
        float lerpProgress = 0;
        while (lerpProgress < 1)
        {
            lerpProgress += lerpIncrement * Time.deltaTime;
            lerpProgress = Mathf.Min(lerpProgress, 1);
            float lerpUpdate = Mathf.Lerp(origHeight, height, lerpProgress);
            CardPlaceholderLayout.preferredHeight = lerpUpdate;
            yield return null;
        }
        expanding = false;
        retracting = false;
    }


}
