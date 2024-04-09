using Abraxas.Stones;

namespace Abraxas.StatBlocks.Controllers
{
    public interface IStatBlockController
    {
        int this[StatValues index] { set; get; }
        string StatsStr { get; }
        StoneType StoneType { get; }
        int Cost { get; set; }
    }
}
