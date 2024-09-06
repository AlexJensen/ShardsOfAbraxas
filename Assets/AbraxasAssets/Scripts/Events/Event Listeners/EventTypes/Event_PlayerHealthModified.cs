using Abraxas.Health.Controllers;
using Abraxas.Manas.Controllers;

namespace Abraxas.Events
{
	public class Event_PlayerHealthModified : IEvent
    {
        #region Properties
        public IPlayerHealthController PlayerHealth { get; set; }
        #endregion

        #region Methods
        public Event_PlayerHealthModified(IPlayerHealthController playerHealth)
        {
            PlayerHealth = playerHealth;
        }
        #endregion
    }
}
