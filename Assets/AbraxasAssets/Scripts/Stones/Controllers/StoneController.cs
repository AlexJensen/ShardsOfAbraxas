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
        public ICardController Card { get; set; }

        public virtual void Initialize(IStoneModel model, ICardController card)
        {
            Model = model;
            Card = card;
        }

        public class Factory : PlaceholderFactory<StoneSO, ICardController, IStoneController> { }
        #endregion

        #region Properties

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
        public StoneType StoneType {get => Model.StoneType; set => Model.StoneType = value;
        }
        #endregion
    }
}