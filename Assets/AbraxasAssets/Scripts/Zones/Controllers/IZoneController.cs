using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using Abraxas.Zones.Models;
using Abraxas.Zones.Views;
using System.Collections;
using System.Collections.Generic;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Controllers
{
    public interface IZoneController
    {
        Player Player { get; }
        IZoneView View { get; }
        Dictionary<StoneType, int> GetTotalCostOfZone();
        ICardController RemoveCard(ICardController card);
        IEnumerator MoveCardToZone(ICardController card, int index = 0);
        ICardController RemoveCard(int index);

        void Initialize<TView, TModel>(TView view, TModel model)
            where TView : IZoneView
            where TModel : IZoneModel;
        void AddCardToZone(ICardController card, int index = 0);
    }
}
