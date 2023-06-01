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
    class CardModel : ICardModel {
        #region Events
        public event Action OnTitleChanged;
        public event Action OnOwnerChanged;
        public event Action OnOriginalOwnerChanged;
        public event Action OnHiddenChanged;
        #endregion

        #region Dependencies
        CardData _data;
        public void Initialize(CardData data, IStatBlockModel statBlockModel, List<IStoneModel> stones)
        {
            _data = data;
            _statBlock = statBlockModel;
            _stones = stones;
        }
        #endregion

        #region Fields
        List<IStoneModel> _stones = new();
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
                if (_data.Owner != value) OnOwnerChanged.Invoke();
                _data.Owner = value;
            }
        }
        public Player OriginalOwner
        {
            get => _data.OriginalOwner;
            set
            {
                if (_data.OriginalOwner != value) OnOriginalOwnerChanged.Invoke();
                _data.OriginalOwner = value;
            }
        }
        public int ImageIndex
        {
            get => _data.ImageIndex;
            set
            {
                _data.ImageIndex = value;
            }
        }
        public List<IStoneModel> Stones => _stones;
        public IStatBlockModel StatBlock => _statBlock;
        public ICellController Cell
        {
            get => _cell;
            set => _cell = value;
        }
        public Point FieldPosition
        {
            get => _fieldPosition;
            set => _fieldPosition = value;
        }
        public IZoneController Zone
        {
            get => _zone;
            set => _zone = value;
        }
        public bool Hidden
        {
            get => _hidden;
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
                foreach (IStoneModel stone in Stones)
                {
                    AddCost(totalCosts, stone.StoneType, stone.Cost);
                }
                AddCost(totalCosts, StatBlock.StoneType, StatBlock.Cost);
                return totalCosts;
            }
        }

        private void AddCost(Dictionary<StoneType, int> totalCosts, StoneType type, int cost)
        {
            if (!totalCosts.ContainsKey(type))
            {
                totalCosts.Add(type, cost);
            }
            else
            {
                totalCosts[type] += cost;
            }
        }
        #endregion
    }
}
