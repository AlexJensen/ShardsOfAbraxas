using Abraxas.Cards.Data;
using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Zones.Controllers;
using System;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Models
{
    class CardModel : ICardModel
    {
        #region Events
        public event Action OnTitleChanged;
        public event Action OnOwnerChanged;
        public event Action OnOriginalOwnerChanged;
        public event Action OnHiddenChanged;
        #endregion

        #region Dependencies
        CardData _data;
        public void Initialize(CardData data, IStatBlockController statBlockModel, List<IStoneController> stones)
        {
            _data = data;
            _statBlock = statBlockModel;
            _stones = stones;
        }
        #endregion

        #region Fields
        List<IStoneController> _stones = new();
        IZoneController _zone;
        ICellController _cell;
        bool _hidden;
        IStatBlockController _statBlock;
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

        public List<IStoneController> Stones
        {
            get => _stones;
            set => _stones = value;
        }
        public IStatBlockController StatBlock
        {
            get => _statBlock;
            set => _statBlock = value;
        }
        public ICellController Cell
        {
            get => _cell;
            set => _cell = value;
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
                foreach (IStoneController stone in Stones)
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
