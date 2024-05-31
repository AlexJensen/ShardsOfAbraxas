using Abraxas.Cards.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Models;
using Zenject;

namespace Abraxas.Stones.Controllers
{


    public class StoneController : IStoneController
    {
        #region Dependencies
        protected IStoneModel Model;

        public virtual void Initialize(IStoneModel model)
        {
            Model = model;
        }

        public class Factory : PlaceholderFactory<StoneDataSO, IStoneController>
        {

        }
        #endregion

        #region Properties
        public ICardController Card { get; set; }
        public int Index { get; set; }

        public int Cost
        {
            get => Model.Cost;
            set => Model.Cost = value;
        }
        public string Info
        {
            get => Model.Info;
            set => Model.Info = value;
        }
        public StoneType StoneType

        {
            get => Model.StoneType;
            set => Model.StoneType = value;
        }
        #endregion
    }
}