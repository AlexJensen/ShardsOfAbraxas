using Abraxas.Manas;

namespace Abraxas.Events
{
    public class ManaModifiedEvent
    {
        #region Properties
        public Mana Mana { get; }
        #endregion

        #region Methods
        public ManaModifiedEvent(Mana mana)
        {
            Mana = mana;
        }
        #endregion
    }
}
