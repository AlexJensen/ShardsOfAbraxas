using System.Collections;
using UnityEngine;

namespace Abraxas.Core
{
    public static class Utilities
    {
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
                runningCoroutines[i] = CoroutineRunner.Instance.StartCoroutine(coroutines[i]);
            }

            foreach (var coroutine in runningCoroutines)
            {
                yield return coroutine;
            }
        }
    }
}