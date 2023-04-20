using Abraxas.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Abraxas.Cards
{
    [RequireComponent(typeof(Card))]
    public class CardMouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Dependencies
        IGameManager _gameManager;
        [Inject]
        public void Construct(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }
        #endregion

        #region Fields
        Card _card;
        #endregion

        #region Properties
        public Card Card => _card = _card != null ? _card : GetComponent<Card>();
        #endregion

        #region Methods
        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(_gameManager.ShowCardDetail(Card));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(_gameManager.HideCardDetail());
        }
        #endregion
    }
}
