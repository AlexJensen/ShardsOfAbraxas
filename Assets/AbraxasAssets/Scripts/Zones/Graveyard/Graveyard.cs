using Abraxas.Behaviours.Cards;
using UnityEngine;

namespace Abraxas.Behaviours.Zones.Graveyards
{
    public class Graveyard : Zone
    {
        public override ZoneManager.Zones ZoneType => ZoneManager.Zones.DEAD;
    }
}