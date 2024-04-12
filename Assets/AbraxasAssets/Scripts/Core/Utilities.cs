using Abraxas.Stones.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private static Dictionary<string, Type> _stoneDataTypeCache;

        static Utilities()
        {
            // Initialize the cache by reflecting over all types implementing IStoneData
            _stoneDataTypeCache = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IStoneData).IsAssignableFrom(type) && !type.IsInterface)
                .ToDictionary(type => type.FullName, type => type);
        }

        public static string GetTypeId<T>() where T : IStoneData
        {
            return typeof(T).FullName;
        }

        public static T CreateInstance<T>(string typeId)
            where T : ScriptableObject
        {
            if (_stoneDataTypeCache.TryGetValue(typeId, out var type))
            {
                return (T)ScriptableObject.CreateInstance(type);
            }
            return default;
        }
    }
}