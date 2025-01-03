﻿using Abraxas.Games.Managers;
using Abraxas.Stones.Targets;
using System.Collections;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Stones.Controllers
{
    /// <summary>
    /// Effect_DrawCardFromLibrary is a stone effect that draws a card from the deck.
    /// </summary>
	public class Effect_DrawCardsFromDeck : EffectStone, ITargetable<Player>
    {
        #region Dependencies
        readonly IGameManager _gameManager;
        [Inject]
        public Effect_DrawCardsFromDeck(IGameManager gameManager) : base()
        {
            _gameManager = gameManager;
        }
        #endregion

        #region Fields
        TargetSO<Player> _target;
        #endregion

        #region Properties
        TargetSO<Player> ITargetable<Player>.Target { get => _target; set => _target = value; }
        #endregion

        #region Methods
        protected override IEnumerator PerformEffect(object[] vals)
        {
            yield return _gameManager.DrawCard(_target.Target);
        }
        #endregion
    }
}
