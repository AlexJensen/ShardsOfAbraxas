using Abraxas.Stones;
using Abraxas.Zones.Fields;
using Abraxas.Cards.Data;
using System.Collections.Generic;
using System.Drawing;

using Zone = Abraxas.Zones.Zones;
using Player = Abraxas.Players.Players;
using System;


namespace Abraxas.Cards.Models
{
    class CardModel : ICardModelReader, ICardModelWriter {
        #region Events
        public event Action OnTitleChanged;
        public event Action OnOwnerChanged;
        public event Action OnOriginalOwnerChanged;
        public event Action OnHiddenChanged;
        #endregion

        #region Fields
        CardData _data;

        List<IStoneController> _stones = new();
        Player _owner;
        Player _originalOwner;
        ICellController _cell;
        Point _fieldPosition;
        Zone _zone;
        bool _hidden;
        #endregion

        #region Properties
        public string Title 
        { 
            get 
            { 
                return _data.Title; 
            } 
            set 
            { 
                _data.Title = value;
                OnTitleChanged.Invoke();
            } 
        }
        public Player Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
                OnOwnerChanged.Invoke();
            }
        }
        public Player OriginalOwner
        {
            get
            {
                return _originalOwner;
            }
            set
            {
                _originalOwner = value;
                OnOriginalOwnerChanged.Invoke();
            }
        }
        public List<IStoneController> Stones
        {
            get
            {
                return _stones;
            }
            set
            {
                _stones = value;
            }
        }
        public StatBlock StatBlock
        {
            get
            {
                return _data.StatBlock;
            }
            set
            {
                _data.StatBlock = value;
            }
        }
        public ICellController Cell
        {
            get
            {
                return _cell;
            }
            set
            {
                _cell = value;
            }
        }
        public Point FieldPosition
        {
            get
            {
                return _fieldPosition;
            }
            set
            {
                _fieldPosition = value;
            }
        }
        public Zone Zone
        {
            get
            {
                return _zone;
            }
            set
            {
                _zone = value;
            }
        }
        public bool Hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                _hidden = value;
                OnHiddenChanged.Invoke();
            }
        }
        public Dictionary<StoneType, int> TotalCosts
        {
            get
            {
                Dictionary<StoneType, int> totalCosts = new();
                foreach (IStoneController stone in Stones)
                {
                    if (!totalCosts.ContainsKey(stone.StoneType))
                    {
                        totalCosts.Add(stone.StoneType, stone.Cost);
                    }
                    else
                    {
                        totalCosts[stone.StoneType] += stone.Cost;
                    }
                }

                return totalCosts;
            }
        }
        #endregion
    }
}