using Abraxas.Zones.Views;

namespace Abraxas.Zones.Decks.Views
{
    class DeckView : ZoneView, IDeckView
    {
        #region Properties
        public override float MoveCardTime => AnimationSettings.MoveCardToDeckTime;
        #endregion
    }
}