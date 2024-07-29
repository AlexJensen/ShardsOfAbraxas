using Abraxas.Stones.Data;
using System;

namespace Abraxas.Stones.Models
{
    class StoneModel : IStoneModel
    {
        #region Events
        public event Action OnCostChanged;
        public event Action OnInfoChanged;
        public event Action OnStoneTypeChanged;
        #endregion

        #region Dependencies
        IStoneData _data;
        public void Initialize(StoneSO data)
        {
            Data = data.Data;
        }
        #endregion

        #region Properties
        public IStoneData Data { get => _data; set => _data = value; }
        public int Cost
        {
            get
            {
                return Data.Cost;
            }
            set
            {
                Data.Cost = value;
                OnCostChanged.Invoke();
            }
        }
        public string Info
        {
            get
            {
                return Data.Info;
            }
            set
            {
                Data.Info = value;
                OnInfoChanged.Invoke();
            }
        }
        public StoneType StoneType
        {
            get
            {
                return Data.StoneType;
            }
            set
            {
                Data.StoneType = value;
                OnStoneTypeChanged.Invoke();
            }
        }
        #endregion
    }
}
