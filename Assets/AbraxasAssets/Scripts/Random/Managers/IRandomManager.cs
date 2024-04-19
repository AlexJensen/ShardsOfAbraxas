using System.Collections;

namespace Abraxas.Random.Managers
{
	public interface IRandomManager
	{
		IEnumerator InitializeRandomSeed();
		int Range(int minvalue, int maxvalue);
	}
}
