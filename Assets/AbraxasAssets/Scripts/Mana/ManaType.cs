using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Game;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Abraxas.Behaviours.Manas
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class ManaType : MonoBehaviour
    {
        StoneData.StoneType type;

        GameManager.Player player;

        [SerializeField]
        TMP_Text amountStr, addStr;

        Animator animator;

        Image image;
        int amount = 0, previousAmount = 0;

        public StoneData.StoneType Type
        {
            get => type;
            set
            {
                type = value;
                Refresh();
            }
        }

        public int Amount
        {
            get => amount;
            set
            {
                if (previousAmount == amount)
                {
                    previousAmount = amount;
                }
                amount = value;
                Refresh();
            }
        }

        public GameManager.Player Player { get => player; set => player = value; }

        private void Awake()
        {
            image = GetComponent<Image>();
            animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            if (previousAmount != amount)
            {
                int change = amount - previousAmount;
                addStr.text = change >= 0 ? "+" + change.ToString() : change.ToString();
                if (Player == GameManager.Player.Player1) TriggerAddManaDown();
                if (Player == GameManager.Player.Player2) TriggerAddManaUp();
                previousAmount = amount;
            }
        }

        public void Refresh()
        {
            StoneData.StoneDetails colorData = DataManager.Instance.GetStoneDetails(Type);
            image.color = colorData.color;
            name = colorData.name;
            amountStr.text = amount.ToString();
        }

        public void TriggerAddManaUp()
        {
            animator.SetTrigger("AddManaUp");
        }

        public void TriggerAddManaDown()
        {
            animator.SetTrigger("AddManaDown");
        }
    }
}