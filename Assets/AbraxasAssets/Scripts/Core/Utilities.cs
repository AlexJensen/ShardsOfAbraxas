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
        public static IEnumerator WaitForCoroutines(MonoBehaviour source, params IEnumerator[] ienumerators)
        {
            if (ienumerators != null & ienumerators.Length > 0)
            {
                Coroutine[] coroutines = new Coroutine[ienumerators.Length];
                for (int i = 0; i < ienumerators.Length; i++)
                    coroutines[i] = source.StartCoroutine(ienumerators[i]);
                for (int i = 0; i < coroutines.Length; i++)
                    yield return coroutines[i];
            }
            else
                yield return null;
        }

        /// <summary>
        /// Checks if the input is of the type indicated by the generic, and shows a debug message if it isn't.
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <param name="source">Object calling this method, used for the debug message if the method fails.</param>
        /// <param name="val">Object to check type.</param>
        /// <returns>True if the types match, false otherwise.</returns>
        public static bool ValidateParam<T>(object source, object val)
        {
            if (val is not T)
            {
                Debug.LogWarning(source.GetType() + ": TypeMismatch Exception! " + val.GetType() + " was not a " + typeof(T) + "!");
                return false;
            }
            return true;
        }
    }
}