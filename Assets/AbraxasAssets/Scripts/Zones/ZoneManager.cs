using Abraxas.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Behaviours.Zones
{
    public class ZoneManager : Singleton<ZoneManager>
    {
        public enum Zones
        {
            DECK, DRAG, HAND, PLAY, DEAD, BANISHED
        }
    }
}
