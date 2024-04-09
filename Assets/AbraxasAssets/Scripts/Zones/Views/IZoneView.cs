using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Models;
using System.Collections;

namespace Abraxas.Zones.Views
{
    public interface IZoneView
    {
        Players.Players Player { get; }
        void AddCardToHolder(ICardController card, int index = 0);
        IEnumerator MoveCardToZone(ICardController card, int index = 0);
        void RemoveCardFromHolder(ICardController view);
        void Initialize<TModel>(TModel model)
            where TModel : IZoneModel;
    }
}
