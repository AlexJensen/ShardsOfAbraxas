using Abraxas.Cards;
using Abraxas.Cards.Views;
using Abraxas.Zones.Controllers;
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

        IZoneController _controller;
        IZoneModel _model;
        public void Initialize<TController, TModel>(TController controller, TModel model)
            where TController : IZoneController
            where TModel : IZoneModel
        {
            _controller = controller;
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
        public IZoneController Controller { get => _controller; set => _controller = value; }
        public IZoneModel Model { get => _model; set => _model = value; }
        public IOverlayManager OverlayManager { get => _overlayManager; set => _overlayManager = value; }
        public Card.Settings.AnimationSettings AnimationSettings { get => _animationSettings; }
        #endregion

        #region Methods
        public virtual IEnumerator MoveCardToZone(ICardView card, int index = 0)
        {
            OverlayManager.SetCard(card);
            yield return card.RectTransformMover.MoveToFitRectangle(_cardHolder, MoveCardTime);
            AddCardToHolder(card, index);
            OverlayManager.ClearCard(card);
        }
        public virtual void AddCardToHolder(ICardView card, int index = 0)
        {
            card.Transform.localScale = Vector3.zero;
            card.Transform.position = transform.position;
            card.Transform.SetParent(CardHolder.transform);
            card.Transform.SetSiblingIndex(index);
        }
        public void RemoveCardFromHolder(ICardView card)
        {
            card.Transform.localScale = Vector3.one;
        }

        
        #endregion
    }
}
