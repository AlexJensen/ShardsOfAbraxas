using Abraxas.Cards.Views;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Models;
using System.Collections;

namespace Abraxas.Zones.Views
{
    public interface IZoneView
    {
        Players.Players Player { get; }
        void AddCardToHolder(ICardView card, int index = 0);
        IEnumerator MoveCardToZone(ICardView card, int index = 0);
        void RemoveCardFromHolder(ICardView view);
        void Initialize<TController, TModel>(TController controller, TModel model)
            where TController : IZoneController
            where TModel : IZoneModel;
    }
}
