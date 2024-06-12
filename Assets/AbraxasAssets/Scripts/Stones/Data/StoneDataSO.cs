using System;
using UnityEngine;

namespace Abraxas.Stones.Data
{
    [CreateAssetMenu(fileName = "New Stone", menuName = "Abraxas/Data/Stone")]
    public abstract class StoneDataSO : ScriptableObject
    {
        public abstract IStoneData Data { get; set; }
        public abstract Type ControllerType { get; set; }
    }
}
