using System.Collections;
using UnityEngine;

namespace Abraxas.Core
{
    /// <summary>
    /// Utilities is a static class that contains general purpose utility methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Runs all listed coroutines in parallel and waits for all of them to finish.
        /// </summary>
        /// <param name="coroutines">List of Coroutines to run.</param>
        /// <returns></returns>
        public static IEnumerator WaitForCoroutines(params IEnumerator[] coroutines)
        {
            if (coroutines == null || coroutines.Length == 0)
            {
                yield return null;
                yield break;
            }

            Coroutine[] runningCoroutines = new Coroutine[coroutines.Length];

            for (int i = 0; i < coroutines.Length; i++)
            {
                if (CoroutineRunner.Instance != null)
                {
                    runningCoroutines[i] = CoroutineRunner.Instance.StartCoroutine(coroutines[i]);
                }
                else
                {
                    Debug.LogWarning("CoroutineRunner instance is null. Running coroutine synchronously for testing.");
                    RunCoroutineToCompletion(coroutines[i]);
                }
            }

            foreach (var coroutine in runningCoroutines)
            {
                if (coroutine != null)
                {
                    yield return coroutine;
                }
            }
        }



        /// <summary>
        /// Runs the entirety of an enumerator instantly including any nested enumerators, primarily used for unit tests
        /// </summary>
        /// <param name="enumerator"></param>
        public static void RunCoroutineToCompletion(IEnumerator enumerator)
        {
            if (enumerator == null) return; 
            while (true)
            {
                bool hasNext = enumerator.MoveNext();
                if (!hasNext) break;

                if (enumerator.Current is IEnumerator nestedEnumerator)
                {
                    RunCoroutineToCompletion(nestedEnumerator);
                }
            }
        }
    }
}