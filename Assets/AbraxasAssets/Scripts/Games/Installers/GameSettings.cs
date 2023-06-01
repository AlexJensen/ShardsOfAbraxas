using System;

namespace Abraxas.Games
{
    public class Game
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            public int Player1CardsToDrawAtStartOfGame;
            public int Player2CardsToDrawAtStartOfGame;
            public int CardsToDrawAtStartOfTurn;
        }
        #endregion
    }
}
