using Abraxas.Manas;
using Abraxas.Manas.Controllers;

namespace Abraxas.Events
{
    public class ManaModifiedEvent
    {
        #region Properties
        public IManaController Mana { get; }
        #endregion

        #region Methods
        public ManaModifiedEvent(IManaController mana)
        {
            Mana = mana;
        }
        #endregion
    }
}
