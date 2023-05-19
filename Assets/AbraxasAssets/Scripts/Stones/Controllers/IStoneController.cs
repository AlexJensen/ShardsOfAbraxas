namespace Abraxas.Stones
{
    public interface IStoneController
    {
        abstract int Cost { get; }
        abstract string Info { get; }
        abstract StoneType StoneType { get; }
    }
}
