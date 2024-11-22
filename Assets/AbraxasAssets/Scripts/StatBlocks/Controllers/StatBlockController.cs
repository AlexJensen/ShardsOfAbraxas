using Abraxas.StatBlocks.Data;
using Abraxas.StatBlocks.Models;
using Abraxas.StatBlocks.Views;
using Abraxas.Stones;

using Zenject;

namespace Abraxas.StatBlocks.Controllers
{
    /// <summary>
    /// StatBlockController facilitates communication between the stat block model and view.
    /// </summary>
    class StatBlockController : IStatBlockController
    {
        #region Dependencies
        IStatBlockModel _model;
        IStatBlockView _view;
        internal void Initialize(IStatBlockModel model, IStatBlockView view)
        {
            _model = model;
            _view = view;
        }
        #endregion

        #region Properties
        public bool ShowSymbols
        {   
            get =>_model.ShowSymbols;
            set => _model.ShowSymbols = value;
        }
        public string StatsStr => ShowSymbols? _model.StatsStrSymbol: _model.StatsStr;
        public StoneType StoneType => _model.StoneType;
        public UnityEngine.Color Color => _view.GetStoneColor(StoneType);

        public int Cost { get => _model.Cost; set => _model.Cost = value; }

        public StatData Stats
        {
            get => _model.Stats;
            set => _model.Stats = value;
        }

        public class Factory : PlaceholderFactory<StatBlockData, IStatBlockView, IStatBlockController>
        {

        }
        #endregion

    }
}
