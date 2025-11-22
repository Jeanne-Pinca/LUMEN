using UnityEngine;
using UnityEngine.Tilemaps; // Required for Tilemaps

public class SecretTilemap : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)]
    public float hiddenTransparency = 0.2f; // 0 is invisible, 0.2 is faintly visible
    public float fadeSpeed = 4f; // How fast it fades in/out

    private Tilemap tilemap;
    private bool isPlayerInside = false;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        
        if (tilemap == null)
        {
            Debug.LogError("SecretTilemap script must be attached to a GameObject with a Tilemap component!");
        }
    }

    void Update()
    {
        if (tilemap == null) return;

        // Determine our target alpha based on whether the player is inside
        float targetAlpha = isPlayerInside ? hiddenTransparency : 1f;

        // Get the current color
        Color currentColor = tilemap.color;

        // Smoothly move the current alpha towards the target alpha
        // Mathf.MoveTowards is perfect for linear fading over time
        float newAlpha = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);

        // Apply the new color
        tilemap.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}