using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoneDetail : MonoBehaviour
{
    [SerializeField]
    TMP_Text costText, info;
    [SerializeField]
    Image costBack;

    public TMP_Text Cost { get => costText; }
    public TMP_Text Info { get => info; }
    public Image CostBack { get => costBack; }
}
