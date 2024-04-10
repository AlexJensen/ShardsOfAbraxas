using Abraxas.StatBlocks.Data;
using Abraxas.StatBlocks.Models;
using Abraxas.StatBlocks.Views;
using Abraxas.Stones;
using Zenject;

namespace Abraxas.StatBlocks.Controllers
{
    class StatBlockController : IStatBlockController
    {
        #region Dependencies
        IStatBlockModel _model;
        internal void Initialize(IStatBlockModel model)
        {
            _model = model;
        }
        #endregion

        #region Properties
        public string StatsStr => _model.StatsStr;
        public StoneType StoneType => _model.StoneType;

        public int Cost { get => _model.Cost; set => _model.Cost = value; }

        public int this[StatValues index] 
        {   
            get => _model[index]; 
            set => _model[index] = value; 
        }

        public class Factory : PlaceholderFactory<StatBlockData, IStatBlockView, IStatBlockController>
        {

        }
        #endregion

    }
}
