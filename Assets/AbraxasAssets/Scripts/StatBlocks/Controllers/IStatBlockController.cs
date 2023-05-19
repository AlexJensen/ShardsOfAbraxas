using Abraxas.Stones;

namespace Abraxas.StatBlocks.Controllers
{
    interface IStatBlockController
    {
        string StatsStr { get; }
        StoneType StoneType { get; }
    }
}
