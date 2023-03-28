using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Abraxas.Behaviours.Zones.Hands
{

    /// <summary>
    /// Controls the height of the placeholder card that appears while dragging a card out of the hand.
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(RectTransform))]
    public class CardPlaceholder : MonoBehaviour
    {
        #region Constants
        const float PLACEHOLDER_SCALE_TIME = .2f;
        #endregion

        #region Fields
        LayoutElement cardPlaceholderLayout;
        RectTransform cardPlaceholderRect;
        public LayoutElement CardPlaceholderLayout => cardPlaceholderLayout = cardPlaceholderLayout != null ? cardPlaceholderLayout : cardPlaceholderLayout = GetComponent<LayoutElement>();
        public RectTransform CardPlaceholderRect => cardPlaceholderRect = cardPlaceholderRect != null ? cardPlaceholderRect : cardPlaceholderRect = (RectTransform)transform;

        int currentIndex = 0, newIndex = 0;
        bool expanding = false, retracting = false;

        [SerializeField]
        float maxHeight;
        #endregion

        #region Unity Methods

        internal void Reset()
        {
            CardPlaceholderLayout.preferredHeight = 0;
            retracting = false;
            expanding = false;
            StopCoroutine(nameof(ScalePlaceholder));
            gameObject.SetActive(false);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the sibling index of the placeholder.
        /// </summary>
        /// <param name="index">New index to set.</param>
        internal void UpdateIndex(int index)
        {
            newIndex = index;
        }

        /// <summary>
        /// Checks the current index against the latest index to determine if the placeholder needs to start moving.
        /// </summary>
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
                        StartCoroutine(ScalePlaceholder(maxHeight, PLACEHOLDER_SCALE_TIME));
                    }
                    else if (!expanding)
                    {
                        retracting = true;
                        StartCoroutine(ScalePlaceholder(0, PLACEHOLDER_SCALE_TIME));
                    }
                }
            }
        }

        internal void SnapToMaxHeight()
        {
            CardPlaceholderLayout.preferredHeight = maxHeight;
        }

        /// <summary>
        /// Starts hiding the placeholder.
        /// </summary>
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

        public IEnumerator ScaleToMaxSize()
        {
            yield return StartCoroutine(ScalePlaceholder(maxHeight, PLACEHOLDER_SCALE_TIME));
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
        #endregion
    }
}