using Abraxas.StatBlocks.Models;
using Abraxas.Stones;

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
        #endregion

    }
}
