using Abraxas.Stones.Models;

namespace Abraxas.Stones.Controllers
{
    public interface IStoneController
    {
        abstract int Cost { get; }
        abstract string Info { get; }
        abstract StoneType StoneType { get; }

        void Initialize(IStoneModel model);
    }
}
