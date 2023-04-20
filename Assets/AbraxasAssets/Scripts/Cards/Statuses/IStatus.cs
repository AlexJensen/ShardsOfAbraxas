namespace Abraxas.Status
{    public interface IStatus
    {
        public abstract void Set(params object[] vals);
        public abstract void Clear(params object[] vals);
    }
}