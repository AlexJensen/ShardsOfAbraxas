using Abraxas.Manas.Controllers;
using Abraxas.Manas.Views;
using Abraxas.Random.Managers;
using Abraxas.Stones;
using Abraxas.Zones.Decks.Controllers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas.Models
{

    /// <summary>
    /// ManaModel is a class that contains all data to represent a mana gameobject.
    /// </summary>
    class ManaModel : IManaModel
    {
        #region Events
        public event Action OnStartOfTurnManaChanged;
        #endregion

        #region Properties
        public Player Player { get; set; }
        public List<ManaType> ManaTypes { get => _manaTypes; set => _manaTypes = value; }
        public Dictionary<StoneType, int> DeckCosts { get => _deckCosts; set => _deckCosts = value; }
        public int TotalDeckCost { get => _totalDeckCost; set => _totalDeckCost = value; }

        public int StartOfTurnMana
        {
            get => _startOfTurnMana; 
            set
            {
                if (_startOfTurnMana != value) OnStartOfTurnManaChanged.Invoke();
                _startOfTurnMana = value;
            }
        }
        #endregion

        #region Fields
        List<ManaType> _manaTypes;
        Dictionary<StoneType, int> _deckCosts;
        int _totalDeckCost = 0;
        int _startOfTurnMana = 0;
        #endregion
    }
}
