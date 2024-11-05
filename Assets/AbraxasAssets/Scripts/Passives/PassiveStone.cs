using Abraxas.Cards.Controllers;
using Abraxas.StatusEffects;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Models;

namespace Abraxas.Passives

{
    public abstract class PassiveStone : StoneController
    {
        #region Dependencies
        public override void Initialize(IStoneModel model, ICardController card)
        {
            base.Initialize(model, card);
            Card.RequestApplyStatusEffect(StatusEffect);
            
        }
        #endregion

        #region Properties
        public abstract IStatusEffect StatusEffect { get; }
        #endregion
    }
}