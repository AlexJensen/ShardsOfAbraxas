using Abraxas.Cards.Data;
using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks.Models;
using Abraxas.Stones;
using Abraxas.Stones.Models;
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
        CardData _data;
        public void Initialize(CardData data, IStatBlockModel statBlockModel)
        {
            _data = data;
            _statBlock = statBlockModel;
           ((IStatBlockModelWriter)statBlockModel).Initialize(data.StatBlock);
        }
        #endregion

        #region Fields
        List<IStoneModel> _stones = new();
        Player _owner;
        Player _originalOwner;
        IZoneController _zone;
        ICellController _cell;
        Point _fieldPosition;
        bool _hidden;
        IStatBlockModel _statBlock;
        #endregion

        #region Properties
        public string Title
        {
            get => _data.Title;
            set
            {
                if (_data.Title != value) OnTitleChanged.Invoke();
                _data.Title = value;
            }
        }
        public Player Owner
        {
            get => _data.Owner;
            set
            {
                if (_owner != value) OnOwnerChanged.Invoke();
                _owner = value;
            }
        }
        public Player OriginalOwner
        {
            get => _data.OriginalOwner;
            set
            {
                if (_originalOwner != value) OnOriginalOwnerChanged.Invoke(); 
                _originalOwner = value;
            }
        }
        public List<IStoneModel> Stones
        {
            get => _stones;
            set
            => _stones = value;
        }
        public IStatBlockModel StatBlock
        {
            get
            => _statBlock;
            
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
                if (!totalCosts.ContainsKey(StatBlock.StoneType))
                {
                    totalCosts.Add(StatBlock.StoneType, StatBlock.Cost);
                }
                else
                {
                    totalCosts[StatBlock.StoneType] += StatBlock.Cost;
                }

                return totalCosts;
            }
        }
        #endregion
    }
}
