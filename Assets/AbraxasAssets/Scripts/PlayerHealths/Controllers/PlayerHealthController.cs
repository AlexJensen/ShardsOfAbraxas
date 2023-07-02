using Abraxas.Health.Models;
using Abraxas.Health.Views;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Controllers
{
    class PlayerHealthController : IPlayerHealthController
    {
        #region Dependencies
        IPlayerHealthModel _model;
        internal void Initialize(IPlayerHealthView view, IPlayerHealthModel model)
        {
            _model = model;
            _model.Player = view.Player;
        }

        public class Factory : PlaceholderFactory<IPlayerHealthView, IPlayerHealthController>
        {

        }
        #endregion

        #region Properties
        public int HP
        {
            get => _model.HP;
            set => _model.HP = value;
        }
        public int MaxHP
        {
            get => _model.MaxHP;
            set => _model.MaxHP = value;
        }
        public Player Player
        {
            get => _model.Player;
            set => _model.Player = value;
        }
        #endregion
    }
}
