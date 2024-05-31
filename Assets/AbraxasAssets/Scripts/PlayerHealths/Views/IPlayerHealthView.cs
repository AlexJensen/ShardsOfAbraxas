using Abraxas.Health.Models;

namespace Abraxas.Health.Views
{
    /// <summary>
    /// IPlayerHealthView is an interface for player health views.
    /// </summary>
    interface IPlayerHealthView
    {
        Players.Players Player { get; }

        void Initialize(IPlayerHealthModel model);
    }
}
