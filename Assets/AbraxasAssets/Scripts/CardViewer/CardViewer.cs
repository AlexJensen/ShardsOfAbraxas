using Abraxas.Cards;
using Abraxas.Stones;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Abraxas.CardViewers
{
    /// <summary>
    /// Displays detailed information about the card currently under the mouse.
    /// </summary>
    public class CardViewer : MonoBehaviour
    {
        #region Dependencies
        Stone.Settings _stoneSettings;

        [Inject]
        public void Construct(Stone.Settings stoneSettings)
        {
            _stoneSettings = stoneSettings;
        }
        #endregion

        #region Fields
        [SerializeField]
        TMP_Text _title, _cost, _attack, _health, _speed;

        [SerializeField]
        Image _image;

        [SerializeField]
        List<StoneViewer> _stones;
        #endregion

        #region methods
        /// <summary>
        /// Displays a card's information in the card detail pane.
        /// </summary>
        /// <param name="card">Card to display.</param>
        public void ShowCardDetails(Card card)
        {
            _title.text = card.Title;
            _cost.text = card.TotalCostText;
            _attack.text = card.StatBlock[StatBlock.StatValues.ATK].ToString();
            _health.text = card.StatBlock[StatBlock.StatValues.DEF].ToString();
            _speed.text = card.StatBlock[StatBlock.StatValues.MV].ToString();
            _image.sprite = card.Image.sprite;
            _image.transform.localScale = card.Image.transform.localScale;

            for (int i = 0; i < _stones.Count; i++)
            {
                if (card.Stones.Count > i && card.Stones[i] is not StatBlock)
                {

                    _stones[i].gameObject.SetActive(true);
                    _stones[i].Cost.text = card.Stones[i].Cost.ToString();
                    _stones[i].CostBack.color = _stoneSettings.GetStoneDetails(card.Stones[i].StoneType).color;
                    _stones[i].Info.text = card.Stones[i].Info;
                }
                else
                {
                    _stones[i].gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
}