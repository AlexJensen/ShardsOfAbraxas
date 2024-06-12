using Abraxas.Games.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.Stones.Controllers
{
    /// <summary>
    /// Effect_DrawCardFromLibrary is a stone effect that draws a card from the deck.
    /// </summary>
	public class Effect_DrawCardFromDeck : EffectStone
    {
        readonly IGameManager _gameManager;

        [Inject]
        public Effect_DrawCardFromDeck(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }
        protected override IEnumerator PerformEffect(object[] vals)
        {
            yield return _gameManager.DrawCard(Card.Owner);
        }
    }
}
