using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using Abraxas.Zones.Views;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Models
{

    public interface IZoneModel
    {
        #region Properties
        List<ICardController> CardList { get; }
        Player Player { get; set; }
        #endregion

        #region Methods
        void AddCard(ICardController card, int index = 0);
        ICardController RemoveCard(int index);
        ICardController RemoveCard(ICardController card);
        IEnumerable<ICardController> GetCardsForPlayer(Player player);
        Dictionary<StoneType, int> GetTotalCostOfZone();
        void Shuffle();
        void Initialize<TView>(TView view) where TView : IZoneView;
        #endregion
    }
}
