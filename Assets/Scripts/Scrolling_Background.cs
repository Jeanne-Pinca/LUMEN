using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    // A reference to the camera that is following the player.
    public Transform cameraTransform;

    // The speed at which the background scrolls. This creates the parallax effect.
    // 0 = background stays still.
    // 0.5 = background moves at half the camera's speed.
    // 1 = background moves with the camera.
    public float parallaxFactor = 0.5f;

    private MeshRenderer meshRenderer;
    private Vector3 cameraStartPosition;
    private float startTextureOffsetX;

    void Start()
    {
        // Get the MeshRenderer component from this GameObject.
        meshRenderer = GetComponent<MeshRenderer>();

        // Store the starting position of the camera.
        cameraStartPosition = cameraTransform.position;

        // Store the initial texture offset from the material.
        startTextureOffsetX = meshRenderer.material.mainTextureOffset.x;
    }

    void LateUpdate()
    {
        // Calculate the distance the camera has moved from its starting position.
        float distanceMoved = cameraTransform.position.x - cameraStartPosition.x;

        // Calculate the new texture offset based on the distance moved and the parallax factor.
        // This creates the scrolling effect.
        float textureOffsetX = startTextureOffsetX + (distanceMoved * parallaxFactor);

        // Create a new Vector2 for the offset. We only want to scroll horizontally (on the X axis).
        Vector2 newOffset = new Vector2(textureOffsetX, 0);

        // Apply the new offset to the material's texture to make it scroll.
        meshRenderer.material.mainTextureOffset = newOffset;
    }
}