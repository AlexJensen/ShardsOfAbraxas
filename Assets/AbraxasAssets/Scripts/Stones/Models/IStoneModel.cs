using System;

namespace Abraxas.Stones.Models
{
    public interface IStoneModel
    {
        #region Events
        public event Action OnCostChanged;
        public event Action OnInfoChanged;
        public event Action OnStoneTypeChanged;
        #endregion

        #region Properties
        int Cost { get; set; }
        string Info { get; set; }
        StoneType StoneType { get; set; }
        #endregion
    }
}
