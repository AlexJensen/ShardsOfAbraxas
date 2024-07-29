namespace Abraxas.Events
{
    public interface IEvent<T> : IEventBase
    {
        T Data { get; set; }
    }

    public interface IEventBase
    {

    }
}
