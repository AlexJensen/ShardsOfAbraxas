﻿using Abraxas.Hands;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Abraxas.Zones.Hands
{
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(RectTransform))]
    public class CardPlaceholder : MonoBehaviour
    {
        #region Dependencies
        Hand.Settings.PlaceholderSettings _settings;
        [Inject]
        public void Construct(Hand.Settings settings)
        {
            _settings = settings.placeholderSettings;
        }
        #endregion

        #region Fields
        LayoutElement _cardPlaceholderLayout;
        RectTransform _cardPlaceholderRect;
        int _currentIndex = 0, _newIndex = 0;
        bool _expanding = false, _retracting = false;
        #endregion

        #region Properties
        public LayoutElement CardPlaceholderLayout => _cardPlaceholderLayout = _cardPlaceholderLayout != null ?
            _cardPlaceholderLayout : GetComponent<LayoutElement>();
        public RectTransform CardPlaceholderRect => _cardPlaceholderRect = _cardPlaceholderRect != null ?
            _cardPlaceholderRect : (RectTransform)transform;
        #endregion

        #region Methods
        internal void Reset()
        {
            CardPlaceholderLayout.preferredHeight = 0;
            _retracting = false;
            _expanding = false;
            StopCoroutine(nameof(ScalePlaceholder));
            gameObject.SetActive(false);
        }

        internal void UpdateIndex(int index)
        {
            _newIndex = index;
        }

        internal void UpdatePosition()
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
                        gameObject.SetActive(true);

                        StartCoroutine(ScalePlaceholder(_settings.maxScale, NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost ? 0 : _settings.scaleToMaxSizeTime));
                    }
                    else if (!_expanding)
                    {
                        _retracting = true;
                        StartCoroutine(ScalePlaceholder(_settings.minScale, NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost ? 0 : _settings.scaleToMaxSizeTime));
                    }
                }
            }
        }

        internal void SnapToMaxHeight()
        {
            CardPlaceholderLayout.preferredHeight = _settings.maxScale;
            _currentIndex = transform.GetSiblingIndex();
        }

        internal void Hide()
        {
            if (CardPlaceholderLayout.preferredHeight == 0)
            {
                Reset();
            }
            else if (!_expanding && !_retracting)
            {
                _retracting = true;
                StartCoroutine(ScalePlaceholder(_settings.minScale, NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost ? 0 : _settings.scaleToMaxSizeTime));
            }
        }

        public IEnumerator ScaleToMaxSize()
        {
            yield return ScalePlaceholder(_settings.maxScale, NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost ? 0 : _settings.scaleToMaxSizeTime);
        }

        IEnumerator ScalePlaceholder(float height, float time)
        {
            if (time == 0) yield break;
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