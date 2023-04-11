using System;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Core
{
    public class Singleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static readonly Lazy<T> _instance = new(() =>
        {
            T[] instance = FindObjectsOfType<T>();
            if (instance.Length == 0)
            {
                Debug.LogError($"An instance of {typeof(T)} is needed in the scene, but was not found!");
                return null;
            }
            if (instance.Length > 1)
            {
                Debug.LogWarning($"Multiple instances of {typeof(T)} were found in the scene, this is not supported! Using the first found instance of {typeof(T)}.");
            }
            return instance[0];
        });

        public static T Instance => _instance.Value;
    }
}