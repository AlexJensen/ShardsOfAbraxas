using Abraxas.Games.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.Stones.Controllers
{
	public class Effect_DrawCardFromLibrary : EffectStone
    {
        readonly IGameManager _gameManager;

        [Inject]
        public Effect_DrawCardFromLibrary(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }
        protected override IEnumerator PerformEffect(object[] vals)
        {
            yield return _gameManager.DrawCard(Card.Owner);
        }
    }
}
