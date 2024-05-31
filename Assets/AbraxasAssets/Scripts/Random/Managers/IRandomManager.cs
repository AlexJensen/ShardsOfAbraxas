using System.Collections;

namespace Abraxas.Random.Managers
{

    /// <summary>
    /// IRandomManager is an interface for managing random number generation.
    /// </summary>
    public interface IRandomManager
    {
        IEnumerator InitializeRandomSeed();
        int Range(int minvalue, int maxvalue);
    }
}
