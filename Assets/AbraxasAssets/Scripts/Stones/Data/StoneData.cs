using System;

namespace Abraxas.Stones.Data
{
    [Serializable]
    public struct StoneData
    {
        public int Cost { get; set; }
        public string Info { get; set; }
        public StoneType StoneType { get; set; }
    }
}
