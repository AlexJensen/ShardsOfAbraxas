using Abraxas.Manas;

namespace Abraxas.Events
{
    public class ManaModifiedEvent
    {
        #region Properties
        public ManaView Mana { get; }
        #endregion

        #region Methods
        public ManaModifiedEvent(ManaView mana)
        {
            Mana = mana;
        }
        #endregion
    }
}
