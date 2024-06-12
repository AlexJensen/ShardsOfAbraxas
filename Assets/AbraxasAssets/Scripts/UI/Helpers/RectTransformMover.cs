using Abraxas.Core;
using System;
using System.Collections;
using UnityEngine;

namespace Abraxas
{
    public class RectTransformMover : MonoBehaviour
    {
        #region Fields
        Vector2 _originalSize;
        #endregion

        #region Properties
        public RectTransform RectTransform => (RectTransform)transform;
        #endregion

        #region Methods

        public void Awake()
        {
            _originalSize = RectTransform.rect.size;
        }
        public IEnumerator ChangeScaleEnumerator(Vector2 size, float time)
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

        public IEnumerator MoveToEnumerator(Vector2 location, float time)
        {
            Vector2 originalLocation = RectTransform.position;
            float lerpIncrement = 1 / time, lerpProgress = 0;
            while (lerpProgress <= 1)
            {
                Vector2 lerpUpdate = Vector2.Lerp(originalLocation, location, lerpProgress);
                RectTransform.position = lerpUpdate;

                lerpProgress += lerpIncrement * Time.deltaTime;
                yield return null;
            }
        }

        public IEnumerator MoveToFitRectangle(RectTransform rectTransform, float time)
        {
            if (time == 0) yield break;
            yield return StartCoroutine(Utilities.WaitForCoroutines(
                ChangeScaleEnumerator(rectTransform.rect.size, time),
                MoveToEnumerator(rectTransform.position, time)));
        }

        public void SetPosition(Vector3 position)
        {
            RectTransform.position = position;
        }

        internal void SetToInitialScale()
        {
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _originalSize.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _originalSize.y);
        }
        #endregion
    }
}
