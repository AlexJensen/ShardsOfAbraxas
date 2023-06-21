using Abraxas.Health.Controllers;
using Abraxas.Health.Models;

namespace Abraxas.Health.Views
{
    interface IPlayerHealthView
    {
        Players.Players Player { get; }

        void Initialize(IPlayerHealthModel model);
    }
}
