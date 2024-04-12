using Abraxas.Cards.Controllers;
using Abraxas.Stones.Models;

namespace Abraxas.Stones.Controllers
{
    public interface IStoneController
    {
        abstract int Cost { get; }
        abstract string Info { get; }
        abstract StoneType StoneType { get; }
        int Index { get; set; }
        ICardController Card { get; set; }

        void Initialize(IStoneModel model);
    }
}
