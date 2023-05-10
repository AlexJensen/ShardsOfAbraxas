namespace Abraxas.Stones
{
    public interface IStoneController
    {
        abstract int Cost { get; set; }
        abstract string Info { get; set; }
        abstract StoneType StoneType { get; set; }
    }
}
