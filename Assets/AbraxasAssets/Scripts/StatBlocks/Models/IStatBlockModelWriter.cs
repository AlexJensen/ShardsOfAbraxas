using Abraxas.StatBlocks.Data;

namespace Abraxas.StatBlocks.Models
{
    public interface IStatBlockModelWriter
    {
        int this[StatValues index] { set; }
        void Initialize(StatBlockData data);
    }
}
