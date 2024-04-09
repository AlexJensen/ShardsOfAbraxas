using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TextureScroller : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Image image;
    private Material instanceMaterial;

    void Awake()
    {
        image = GetComponent<Image>();

        // Create a material instance
        instanceMaterial = new Material(image.material);
        image.material = instanceMaterial;
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        // Apply the offset to the instance material
        instanceMaterial.mainTextureOffset = new Vector2(offset % 1, 0);
    }

    void OnDestroy()
    {
        // Clean up the material instance when this object is destroyed
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }
}