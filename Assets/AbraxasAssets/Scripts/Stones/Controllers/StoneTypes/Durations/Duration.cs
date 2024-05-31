
using Abraxas.Cards.Controllers;
using Abraxas.Events;
using Abraxas.Players.Managers;
using System;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [Serializable]
    public abstract class Duration
    {
        public abstract bool IsExpired(GameStateEnteredEvent eventData, ICardController card, IPlayerManager playerManager);
    }
}
