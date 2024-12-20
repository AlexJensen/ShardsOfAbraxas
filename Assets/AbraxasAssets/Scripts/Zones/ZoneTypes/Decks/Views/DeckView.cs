using Abraxas.Zones.Views;
using Unity.Netcode;

namespace Abraxas.Zones.Decks.Views
{
    class DeckView : ZoneView, IDeckView
    {
        #region Properties

        protected override float MoveCardTime => NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost ? 0 : AnimationSettings.MoveCardToDeckTime;
        #endregion
    }
}