using Abraxas.Zones.Views;
using Unity.Netcode;

namespace Abraxas.Zones.Decks.Views
{
    class DeckView : ZoneView, IDeckView
    {
        #region Properties
        public override float MoveCardTime => NetworkManager.Singleton.IsServer? 0: AnimationSettings.MoveCardToDeckTime;
        #endregion
    }
}