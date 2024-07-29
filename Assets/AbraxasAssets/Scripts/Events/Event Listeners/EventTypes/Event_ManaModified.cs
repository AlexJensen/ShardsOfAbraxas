using Abraxas.Manas.Controllers;

namespace Abraxas.Events
{
	public class Event_ManaModified : IEvent<IManaController>
    {
        #region Properties
        public IManaController Data { get; set; }
        #endregion

        #region Methods
        public Event_ManaModified(IManaController mana)
        {
            Data = mana;
        }
        #endregion
    }
}
