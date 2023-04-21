using Abraxas.Core;
using System.Collections;
using UnityEngine;

namespace Abraxas
{
    public class RectTransformMover : MonoBehaviour
    {
        #region Properties
        public RectTransform RectTransform => (RectTransform)transform;
        #endregion

        #region Methods
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
            yield return StartCoroutine(Utilities.WaitForCoroutines(this,
                ChangeScaleEnumerator(rectTransform.rect.size, time),
                MoveToEnumerator(rectTransform.position, time)));
        }
        #endregion
    }
}
