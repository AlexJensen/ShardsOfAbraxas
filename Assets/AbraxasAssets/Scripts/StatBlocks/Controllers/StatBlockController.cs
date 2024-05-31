using Abraxas.StatBlocks.Data;
using Abraxas.StatBlocks.Models;
using Abraxas.StatBlocks.Views;
using Abraxas.Stones;

using NSubstitute.Routing.Handlers;
using UnityEngine;

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
        internal void Initialize(IStatBlockModel model)
        {
            _model = model;
        }
        #endregion

        #region Properties
        public string StatsStr => _model.StatsStr;
        public StoneType StoneType => _model.StoneType;

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
