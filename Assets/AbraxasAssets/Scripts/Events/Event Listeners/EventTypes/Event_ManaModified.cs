using Abraxas.Manas.Controllers;

namespace Abraxas.Events
{
	public class Event_ManaModified : IEvent
    {
        #region Properties
        public IManaController Mana { get; set; }
        #endregion

        #region Methods
        public Event_ManaModified(IManaController mana)
        {
            Mana = mana;
        }
        #endregion
    }
}
