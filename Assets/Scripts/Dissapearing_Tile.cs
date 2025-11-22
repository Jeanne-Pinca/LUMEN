using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DisappearingTilemap : MonoBehaviour
{
    [Header("Settings")]
    public float timeBeforeDisappear = 0.5f;
    public float timeToRespawn = 2f;
    
    [Header("Visuals")]
    public float alphaWhenDisappeared = 0f;
    public Color warningColor = Color.red;

    private Tilemap tilemap;
    private Collider2D tileCollider;
    private Color originalColor;
    private bool isDisappearing = false;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tileCollider = GetComponent<Collider2D>();

        if (tilemap == null)
        {
            Debug.LogError("MISSING TILEMAP: Script is attached to " + gameObject.name + " but no Tilemap component found!");
        }
        
        if (tileCollider == null)
        {
             Debug.LogError("MISSING COLLIDER: " + gameObject.name + " needs a TilemapCollider2D!");
        }

        if (tilemap != null) originalColor = tilemap.color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Check if it hit the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player touched the platform!"); // <--- CHECK CONSOLE FOR THIS

            if (!isDisappearing)
            {
                // --- FIX: Remove the unreliable position check for now ---
                // Instead, we check the "Normal" of the collision.
                // If the collision points UP, it means the player landed ON TOP.
                
                Vector2 contactNormal = collision.contacts[0].normal;
                
                // -0.5 means the player is hitting the top of the platform (pushing down)
                if (contactNormal.y < -0.5f) 
                {
                    Debug.Log("Player is on top! Starting disappear.");
                    StartCoroutine(DisappearRoutine());
                }
                else
                {
                    Debug.Log("Player hit the side or bottom. Ignoring.");
                }
            }
        }
    }

    private IEnumerator DisappearRoutine()
    {
        isDisappearing = true;

        // WARNING PHASE
        tilemap.color = warningColor;
        yield return new WaitForSeconds(timeBeforeDisappear);

        // VANISH PHASE
        tileCollider.enabled = false;
        Color transparentColor = originalColor;
        transparentColor.a = alphaWhenDisappeared;
        tilemap.color = transparentColor;

        // RESPAWN PHASE
        yield return new WaitForSeconds(timeToRespawn);

        tileCollider.enabled = true;
        tilemap.color = originalColor;
        isDisappearing = false;
    }
}