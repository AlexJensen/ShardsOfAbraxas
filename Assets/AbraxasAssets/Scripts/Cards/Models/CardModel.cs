using Abraxas.Cards.Data;
using Abraxas.Cells.Controllers;
using Abraxas.Stones;
using Abraxas.Zones.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Models
{
    class CardModel : ICardModelReader, ICardModelWriter {
        #region Events
        public event Action OnTitleChanged;
        public event Action OnOwnerChanged;
        public event Action OnOriginalOwnerChanged;
        public event Action OnHiddenChanged;
        #endregion

        #region Dependencies
        public void Initialize(CardData data)
        {
            _data = data;
        }
        #endregion

        #region Fields
        CardData _data;

        List<IStoneController> _stones = new();
        Player _owner;
        Player _originalOwner;
        IZoneController _zone;
        ICellController _cell;
        Point _fieldPosition;
        bool _hidden;
        #endregion

        #region Properties
        public string Title
        {
            get => _data.Title;
            set
            {
                _data.Title = value;
                OnTitleChanged.Invoke();
            }
        }
        public Player Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                OnOwnerChanged.Invoke();
            }
        }
        public Player OriginalOwner
        {
            get => _originalOwner;
            set
            {
                _originalOwner = value;
                OnOriginalOwnerChanged.Invoke();
            }
        }
        public List<IStoneController> Stones
        {
            get => _stones;
            set
            => _stones = value;
        }
        public StatBlock StatBlock
        {
            get
            => _data.StatBlock;
            set
            {
                _data.StatBlock = value;
            }
        }
        public ICellController Cell
        {
            get
            => _cell;
            set
            => _cell = value;
        }
        public Point FieldPosition
        {
            get
            => _fieldPosition;
            set => _fieldPosition = value;
        }
        public IZoneController Zone
        {
            get
            => _zone;
            set => _zone = value;
        }
        public bool Hidden
        {
            get
            => _hidden;
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
