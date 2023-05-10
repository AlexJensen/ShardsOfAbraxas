using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Core
{

    /// <summary>
    /// Utilities stores commonly used helper methods that don't require specific gameobjects or monobehaviors to function.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Runs multiple coroutines at once, and waits till every coroutine ran is finished.
        /// </summary>
        /// <param name="source">Coroutines are started from this Monobehavior.</param>
        /// <param name="ienumerators">List of all IEnumerators to create coroutines for.</param>
        /// <returns></returns>
        public static IEnumerator WaitForCoroutines(params IEnumerator[] coroutines)
        {
            if (coroutines == null || coroutines.Length == 0)
            {
                yield return null;
                yield break;
            }

            var remainingCoroutines = new List<IEnumerator>(coroutines);
            while (remainingCoroutines.Count > 0)
            {
                var currentCoroutine = remainingCoroutines[0];
                remainingCoroutines.RemoveAt(0);
                yield return currentCoroutine;

                for (int i = remainingCoroutines.Count - 1; i >= 0; i--)
                {
                    if (!remainingCoroutines[i].MoveNext())
                    {
                        remainingCoroutines.RemoveAt(i);
                    }
                }
            }
        }
    }
}