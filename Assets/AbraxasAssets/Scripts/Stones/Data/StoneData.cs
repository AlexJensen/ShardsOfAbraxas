using System;
using UnityEngine;

namespace Abraxas.Stones
{
    [Serializable]
    public abstract class StoneData : ScriptableObject
    {
        public int Cost { get; set; }
        public string Info { get; set; }
        public StoneType StoneType { get; set; }
    }
}
