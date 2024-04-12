using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Stones.Data
{
    public abstract class TriggerStoneDataSO : StoneDataSO
    {
        [HideInInspector]
        public List<int> Indexes;
    }
}
