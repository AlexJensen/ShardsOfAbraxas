using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Abraxas.Behaviours.CardViewer
{
    public class StoneViewer : MonoBehaviour
    {
        [SerializeField]
        TMP_Text _cost, _info;
        [SerializeField]
        Image _costBack;

        public TMP_Text Cost { get => _cost; }
        public TMP_Text Info { get => _info; }
        public Image CostBack { get => _costBack; }
    }
}