using Abraxas.StatBlocks.Data;

namespace Abraxas.StatBlocks.Models
{
    public interface IStatBlockModel : IStatBlockModelReader, IStatBlockModelWriter
    {
        new int this[StatValues index] { set; get; }
    }
}
