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
        TMP_Text _title, _cost, _attack, _health, _speed, _range;

        [SerializeField]
        Image _image;

        [SerializeField]
        List<StoneViewer> _stones;
        #endregion

        #region methods
        public void ShowCardDetails(ICardController card)
        {
            _title.text = card.Title;
            _cost.text = card.GetCostText();

            _attack.text = card.StatBlock.Stats.ATK.ToString();
            _health.text = card.StatBlock.Stats.DEF.ToString();
            _speed.text = card.StatBlock.Stats.SPD.ToString();
            _range.text = card.StatBlock.Stats.RNG.ToString();

            _image.sprite = card.ImageManipulator.Image.sprite;
            _image.transform.localScale = card.ImageManipulator.Image.transform.localScale;

            for (int i = 0; i < _stones.Count; i++)
            {
                if (card.Stones.Count > i)
                {
                    _stones[i].gameObject.SetActive(true);
                    _stones[i].Cost.text = card.Stones[i].Cost.ToString();
                    _stones[i].CostBack.color = _stoneSettings.GetStoneTypeDetails(card.Stones[i].StoneType).color;
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