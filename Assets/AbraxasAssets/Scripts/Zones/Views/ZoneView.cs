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
        RectTransform _cardHolder;
        [SerializeField]
        Player _player;
        #endregion

        #region Properties
        public Transform CardHolder { get => _cardHolder; }
        public Player Player { get => _player; }
        public abstract float MoveCardTime { get; }
        public IOverlayManager OverlayManager { get => _overlayManager; set => _overlayManager = value; }
        public Card.Settings.AnimationSettings AnimationSettings { get => _animationSettings; }
        protected IZoneModel Model { get => _model;}
        #endregion

        #region Methods
        public virtual IEnumerator MoveCardToZone(ICardController card, int index = 0)
        {
            OverlayManager.SetCard(card);
            yield return card.RectTransformMover.MoveToFitRectangle(_cardHolder, MoveCardTime);
            AddCardToHolder(card, index);
            OverlayManager.ClearCard(card);
        }
        public virtual void AddCardToHolder(ICardController card, int index = 0)
        {
            card.TransformManipulator.Transform.localScale = Vector3.zero;
            card.TransformManipulator.Transform.position = transform.position;
            card.TransformManipulator.Transform.SetParent(CardHolder.transform);
            card.TransformManipulator.Transform.SetSiblingIndex(index);
        }
        public virtual void RemoveCardFromHolder(ICardController card)
        {
            card.TransformManipulator.Transform.localScale = Vector3.one;
        }
        #endregion
    }
}
