using Abraxas.Cards;
using Abraxas.Cards.Controllers;
using Abraxas.Zones.Models;
using Abraxas.Zones.Overlays.Managers;
using System.Collections;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Views
{

    public abstract class ZoneView : MonoBehaviour, IZoneView
    {
        #region Dependencies
        IOverlayManager _overlayManager;
        Card.Settings.AnimationSettings _animationSettings;
        [Inject]
        public void Construct(Card.Settings cardSettings, IOverlayManager overlayManager)
        {
            _animationSettings = cardSettings.animationSettings;
            _overlayManager = overlayManager;
        }

        IZoneModel _model;
        public void Initialize<TModel>(TModel model)
            where TModel : IZoneModel
        {
            _model = model;
        }
        #endregion

        #region Fields
        [SerializeField]
        RectTransform _cardHolder, _cardMoveTo;
        [SerializeField]
        Player _player;
        #endregion

        #region Properties
        public Transform CardHolder { get => _cardHolder; }
        public Player Player { get => _player; }
        
        
        public Card.Settings.AnimationSettings AnimationSettings { get => _animationSettings; }

        protected abstract float MoveCardTime { get; }
        protected IOverlayManager OverlayManager { get => _overlayManager; set => _overlayManager = value; }
        protected IZoneModel Model { get => _model; }
        #endregion

        #region Methods
        public virtual IEnumerator MoveCardToZone(ICardController card, int index = 0)
        {
            OverlayManager.SetCard(card);
            yield return card.RectTransformMover.MoveToFitRectangle(_cardMoveTo, MoveCardTime);
            AddCardToHolder(card, index);
            OverlayManager.ClearCard(card);
        }
        public virtual void AddCardToHolder(ICardController card, int index = 0)
        {
            card.TransformManipulator.Transform.SetParent(CardHolder.transform);
            card.TransformManipulator.Transform.SetSiblingIndex(index);
            card.SetToInitialScale();
        }
        public virtual void RemoveCardFromHolder(ICardController card)
        {
            card.TransformManipulator.Transform.position = _cardMoveTo.position;
        }
        #endregion
    }
}
