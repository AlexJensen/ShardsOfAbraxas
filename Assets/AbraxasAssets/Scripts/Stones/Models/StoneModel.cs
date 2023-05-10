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

        #region Fields
        StoneData _data;
        #endregion
        #region Properties


        public int Cost
        {
            get
            {
                return _data.Cost;
            }
            set 
            {
                _data.Cost = value;
                OnCostChanged.Invoke();
            }
        }
        public string Info
        {
            get
            {
                return _data.Info;
            }
            set
            {
                _data.Info = value;
                OnCostChanged.Invoke();
            }
        }
        public StoneType StoneType
        {
            get
            {
                return _data.StoneType;
            }
            set
            {
                _data.StoneType = value;
                OnStoneTypeChanged.Invoke();
            }
        }
        #endregion
    }
}
