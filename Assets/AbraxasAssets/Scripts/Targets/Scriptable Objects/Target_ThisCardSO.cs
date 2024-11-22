using Abraxas.Cards.Controllers;
using Abraxas.Stones.Controllers;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Target ThisCard", menuName = "Abraxas/Data/StoneData/Targets/ThisCard")]
    class Target_ThisCardSO : TargetSO<ICardController>
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
                // do not override the target
            }
        }

        public override void Initialize(IStoneController stone)
        {
            _cardTarget = stone.Card;
        }
    }
}
