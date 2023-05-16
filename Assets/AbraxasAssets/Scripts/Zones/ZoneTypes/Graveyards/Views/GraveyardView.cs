using Abraxas.Zones.Views;

namespace Abraxas.Zones.Graveyards.Views
{
    class GraveyardView : ZoneView, IGraveyardView
    {
        #region Properties
        public override float MoveCardTime => AnimationSettings.MoveCardToGraveyardTime;
        #endregion
    }
}