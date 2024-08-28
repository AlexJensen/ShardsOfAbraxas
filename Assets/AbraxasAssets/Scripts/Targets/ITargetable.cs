namespace Abraxas.Stones.Targets
{
    interface ITargetable<T>
    {
        TargetSO<T> Target { get; set; }
    }
}
