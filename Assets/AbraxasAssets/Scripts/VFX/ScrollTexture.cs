using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Players;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
public class TextureScroller : MonoBehaviour, IGameEventListener<ActivePlayerChangedEvent>
{
    public float scrollSpeed = 0.5f;
    private Image image;
    private Material instanceMaterial;
    private bool isPlayer1 = true;

    IEventManager _eventManager;
    [Inject]
    public void Construct(IEventManager eventManager)
    {
        _eventManager = eventManager;
        eventManager.AddListener(typeof(ActivePlayerChangedEvent), this);
    }

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
        instanceMaterial.mainTextureOffset = new Vector2(isPlayer1 ? ((1 - offset) % 1) : offset % 1, 0);
    }

    void OnDestroy()
    {
        // Clean up the material instance when this object is destroyed
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
        _eventManager.RemoveListener(typeof(ActivePlayerChangedEvent), this);
    }

    public IEnumerator OnEventRaised(ActivePlayerChangedEvent eventData)
    {
        isPlayer1 = eventData.Player == Players.Player1;
        yield break;
    }

    public bool ShouldReceiveEvent(ActivePlayerChangedEvent eventData)
    {
        return true;
    }
}
