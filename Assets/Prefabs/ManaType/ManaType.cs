using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ManaType : MonoBehaviour
{
    public GameData data;
    public GameData.StoneTypes type;

    Image image;

    private void Update()
    {
            image = GetComponent<Image>();

            GameData.StoneColor colorData = data.stoneColors.Find(x => x.type == type);
            image.color = colorData.color;
            name = colorData.name;
    }
}
