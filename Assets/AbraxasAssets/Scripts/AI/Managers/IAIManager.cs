using System.Collections;

namespace Abraxas.AI.Managers
{
    public interface IAIManager
    {
        bool IsPlayer1AI { get; set; }
        bool IsPlayer2AI { get; set; }

        IEnumerator DeterminePlay();
    }
}
