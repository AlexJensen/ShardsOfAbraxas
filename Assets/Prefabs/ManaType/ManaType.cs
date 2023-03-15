using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ManaType : MonoBehaviour
{
    StoneData.StoneType type;

    [SerializeField]
    TMP_Text amountStr;

    Image image;
    int amount;

    public StoneData.StoneType Type 
    { 
        get => type; 
        set 
        {
            type = value;
            Refresh();
        } 
    }

    public int Amount { get => amount;
        set
        {
            amount = value;
            Refresh();
        }
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Refresh()
    {
        StoneData.StoneColor colorData = Game.Instance.stoneData.stoneColors.Find(x => x.type == Type);
        image.color = colorData.color;
        name = colorData.name;
        amountStr.text = amount.ToString();
    }
}
