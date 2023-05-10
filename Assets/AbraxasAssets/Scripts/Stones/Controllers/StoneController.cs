using Abraxas.Cards.Controllers;
using Abraxas.Stones.Models;

namespace Abraxas.Stones.Controllers
{

    class StoneController: IStoneController
    {
        #region Dependencies
        readonly IStoneModel _model;
        public StoneController(IStoneModel model)
        {
            _model = model;
        }
        #endregion

        #region Properties
        public ICardController Card { get; set; }

        public int Cost
        {
            get => _model.Cost;
            set => _model.Cost = value;
        }
        public string Info
        {
            get => _model.Info;
            set => _model.Info = value;
        }
        public StoneType StoneType
        { 
            get => _model.StoneType;
            set => _model.StoneType = value;
        }
        #endregion
    }
}