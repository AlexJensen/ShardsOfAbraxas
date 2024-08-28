using Abraxas.Cards.Controllers;
using Abraxas.Stones.Controllers;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Target ThisCard", menuName = "Abraxas/StoneData/Targets/ThisCard")]
    class Target_ThisCard : TargetSO<ICardController>
    {
        ICardController _cardTarget;
        public override ICardController Target
        {
            get
            {
                return _cardTarget;
            }
            set
            {
                //
            }
        }

        public override void Initialize(IStoneController stone)
        {
            _cardTarget = stone.Card;
            base.Initialize(stone);
        }
    }
}
