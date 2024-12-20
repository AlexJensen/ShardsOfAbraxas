using Abraxas.Zones.Views;
using Unity.Netcode;

namespace Abraxas.Zones.Graveyards.Views
{
    class GraveyardView : ZoneView, IGraveyardView
    {
        #region Properties
        protected override float MoveCardTime => NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost ? 0 : AnimationSettings.MoveCardToGraveyardTime;
        #endregion
    }
}