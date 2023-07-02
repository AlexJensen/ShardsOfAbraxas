using System;
using Player = Abraxas.Players.Players;
namespace Abraxas.Health.Models
{
    public interface IPlayerHealthModel
    {
        event Action OnHealthChanged;
        event Action OnMaxHealthChanged;

        int HP { get; set; }
        int MaxHP { get; set; }
        Player Player { get; set; }

        void Initialize();
    }
}
