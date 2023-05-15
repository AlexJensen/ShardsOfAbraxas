using System;
using UnityEngine;

namespace Abraxas.Stones
{
    [Serializable]
    public struct StoneData
    {
        public int Cost { get; set; }
        public string Info { get; set; }
        public StoneType StoneType { get; set; }
    }
}
