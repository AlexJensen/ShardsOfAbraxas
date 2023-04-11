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
        [SerializeField]
        float _maxHeight;

        LayoutElement _cardPlaceholderLayout;
        RectTransform _cardPlaceholderRect;
        int _currentIndex = 0, _newIndex = 0;
        bool _expanding = false, _retracting = false;
        #endregion

        #region Properties
        public LayoutElement CardPlaceholderLayout => _cardPlaceholderLayout ??=  GetComponent<LayoutElement>();
        public RectTransform CardPlaceholderRect => _cardPlaceholderRect ??=(RectTransform)transform;


        #endregion

        #region Unity Methods

        internal void Reset()
        {
            CardPlaceholderLayout.preferredHeight = 0;
            _retracting = false;
            _expanding = false;
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
            _newIndex = index;
        }

        /// <summary>
        /// Checks the current index against the latest index to determine if the placeholder needs to start moving.
        /// </summary>
        internal void CheckPosition()
        {
            if (_newIndex != _currentIndex)
            {
                if (!_retracting)
                {
                    if (CardPlaceholderLayout.preferredHeight == 0)
                    {
                        _expanding = true;
                        CardPlaceholderLayout.transform.SetSiblingIndex(_newIndex);
                        _currentIndex = _newIndex;
                        StartCoroutine(ScalePlaceholder(_maxHeight, PLACEHOLDER_SCALE_TIME));
                    }
                    else if (!_expanding)
                    {
                        _retracting = true;
                        StartCoroutine(ScalePlaceholder(0, PLACEHOLDER_SCALE_TIME));
                    }
                }
            }
        }

        internal void SnapToMaxHeight()
        {
            CardPlaceholderLayout.preferredHeight = _maxHeight;
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
            else if (!_expanding && !_retracting)
            {
                _retracting = true;
                StartCoroutine(ScalePlaceholder(0, PLACEHOLDER_SCALE_TIME));
            }
        }

        public IEnumerator ScaleToMaxSize()
        {
            yield return StartCoroutine(ScalePlaceholder(_maxHeight, PLACEHOLDER_SCALE_TIME));
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
            _expanding = false;
            _retracting = false;
        }
        #endregion
    }
}