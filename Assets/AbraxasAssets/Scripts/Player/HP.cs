using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Abraxas.Behaviours.Game;

namespace Abraxas.Behaviours.Player
{
    public class HP : MonoBehaviour
    {
        [SerializeField]
        TMP_Text HPStr, addHPStr, addHPMaxStr;

        [SerializeField]
        GameManager.Player player;

        Animator animator;

        [SerializeField]
        int hPValue, hPMaxValue;
        int previousHPValue, previousHPMaxValue;

        public GameManager.Player Player { get => player; set => player = value; }

        public int HPValue
        {
            get => hPValue; 
            set
            {
                if (previousHPValue == hPValue)
                {
                    previousHPValue = hPValue;
                }
                hPValue = value;
                Refresh();
            }
        }
        public int HPMaxValue
        {
            get => hPMaxValue;
            set
            {
                if (previousHPValue == hPValue)
                {
                    previousHPMaxValue = hPMaxValue;
                }
                hPMaxValue = value;
                Refresh();
            }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            if (previousHPValue != hPValue)
            {
                int hPChange = hPValue - previousHPValue;
                addHPStr.text = hPChange >= 0 ? "+" + hPChange.ToString() : hPChange.ToString();
                if (Player == GameManager.Player.Player1) SetAnimationTrigger("AddHPDown");
                if (Player == GameManager.Player.Player2) SetAnimationTrigger("AddHPUp");
                previousHPValue = hPValue;
            }

            if (previousHPMaxValue != hPMaxValue)
            {
                int hPMaxChange = hPMaxValue - previousHPMaxValue;
                addHPMaxStr.text = hPMaxChange >= 0 ? "+" + hPMaxChange.ToString() : hPMaxChange.ToString();
                if (Player == GameManager.Player.Player1) SetAnimationTrigger("AddMaxHPDown");
                if (Player == GameManager.Player.Player2) SetAnimationTrigger("AddMaxHPUp");
                previousHPMaxValue = hPMaxValue;
            }
        }

        public void Refresh()
        {
            HPStr.text = hPValue.ToString() + "/" + hPMaxValue.ToString();
        }

        public void SetAnimationTrigger(string trigger)
        {
            animator.SetTrigger(trigger);
        }
    }
}
