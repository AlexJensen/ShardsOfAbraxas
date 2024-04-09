using Abraxas.Cards.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Models;
using Zenject;

namespace Abraxas.Stones.Controllers
{

	public class StoneController: IStoneController
    {
        #region Dependencies
        protected IStoneModel _model;
        public void Initialize(IStoneModel model)
        {
            _model = model;
        }

        public class Factory : PlaceholderFactory<StoneDataSO, IStoneController>
        {

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