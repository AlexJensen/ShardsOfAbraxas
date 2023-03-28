using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Stones;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Abraxas.Behaviours.CardViewer
{
    /// <summary>
    /// Displays detailed information about the card currently under the mouse.
    /// </summary>
    public class CardViewer : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        TMP_Text title, cost, attack, health, speed;

        [SerializeField]
        Image image;

        [SerializeField]
        List<StoneViewer> stones;
        #endregion

        #region methods
        /// <summary>
        /// Displays a card's information in the card detail pane.
        /// </summary>
        /// <param name="card">Card to display.</param>
        public void ShowCardDetails(Card card)
        {
            title.text = card.Title;
            cost.text = card.TotalCostText;
            attack.text = card.StatBlock[StatBlock.StatValues.ATK].ToString();
            health.text = card.StatBlock[StatBlock.StatValues.DEF].ToString();
            speed.text = card.StatBlock[StatBlock.StatValues.MV].ToString();
            image.sprite = card.Image.sprite;
            image.transform.localScale = card.Image.transform.localScale;

            for (int i = 0; i < stones.Count; i++)
            {
                if (card.Stones.Count > i && card.Stones[i] is not StatBlock)
                {

                    stones[i].gameObject.SetActive(true);
                    stones[i].Cost.text = card.Stones[i].Cost.ToString();
                    stones[i].CostBack.color = DataManager.Instance.GetStoneDetails(card.Stones[i].StoneType).color;
                    stones[i].Info.text = card.Stones[i].Info;
                }
                else
                {
                    stones[i].gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
}