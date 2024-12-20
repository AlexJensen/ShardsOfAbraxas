using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TextureScroller : MonoBehaviour
{
    public Vector2 scrollSpeed = new(0.5f, 0.0f);
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
        float offsetX = Time.time * scrollSpeed.x;
        float offsetY = Time.time * scrollSpeed.y;
        // Apply the offset to the instance material
        instanceMaterial.mainTextureOffset = new Vector2(offsetX % 1, offsetY % 1);
    }

    public void SetScrollDirection(Vector2 direction)
    {
        scrollSpeed = direction;
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
