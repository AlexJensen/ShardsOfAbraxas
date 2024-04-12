using Abraxas.Zones.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.Stones.Controllers
{
    public class Effect_DrawCardFromLibrary : EffectStone
    {
        readonly IZoneManager _zoneManager;

        [Inject]
        public Effect_DrawCardFromLibrary(IZoneManager zoneManager)
        {
            _zoneManager = zoneManager;
        }
        public override IEnumerator TriggerEffect(object[] vals)
        {
            yield return _zoneManager.MoveCardsFromDeckToHand(Card.Owner, 1);
        }
    }
}
