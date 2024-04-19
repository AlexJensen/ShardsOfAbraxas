using Abraxas.Zones.Views;
using Unity.Netcode;

namespace Abraxas.Zones.Graveyards.Views
{
    class GraveyardView : ZoneView, IGraveyardView
    {
        #region Properties
        public override float MoveCardTime => NetworkManager.Singleton.IsServer ? 0 : AnimationSettings.MoveCardToGraveyardTime;
        #endregion
    }
}