using Abraxas.Core;

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
