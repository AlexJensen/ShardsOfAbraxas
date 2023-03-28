using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Abraxas.Behaviours.CardViewer
{
    public class StoneViewer : MonoBehaviour
    {
        [SerializeField]
        TMP_Text costText, info;
        [SerializeField]
        Image costBack;

        public TMP_Text Cost { get => costText; }
        public TMP_Text Info { get => info; }
        public Image CostBack { get => costBack; }
    }
}