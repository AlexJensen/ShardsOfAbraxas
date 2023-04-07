using System;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Core
{
    public class Singleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
        {
            T instance = FindObjectOfType<T>();
            if (instance == null)
            {
                Debug.LogError($"An instance of {typeof(T)} is needed in the scene, but was not found!");
            }
            return instance;
        });

        public static T Instance => _instance.Value;
    }
}