using Abraxas.Cards.Controllers;
using Abraxas.StatBlocks;
using Abraxas.Stones;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Abraxas.CardViewers
{
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
        public void ShowCardDetails(ICardController card)
        {
            _title.text = card.Title;
            _cost.text = card.View.GetCostText();
            _attack.text = card.StatBlock[StatValues.ATK].ToString();
            _health.text = card.StatBlock[StatValues.DEF].ToString();
            _speed.text = card.StatBlock[StatValues.MV].ToString();
            _image.sprite = card.View.Image.sprite;
            _image.transform.localScale = card.View.Image.transform.localScale;

            for (int i = 0; i < _stones.Count; i++)
            {
                if (card.Model.Stones.Count > i)
                {
                    _stones[i].gameObject.SetActive(true);
                    _stones[i].Cost.text = card.Model.Stones[i].Cost.ToString();
                    _stones[i].CostBack.color = _stoneSettings.GetStoneDetails(card.Model.Stones[i].StoneType).color;
                    _stones[i].Info.text = card.Model.Stones[i].Info;
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