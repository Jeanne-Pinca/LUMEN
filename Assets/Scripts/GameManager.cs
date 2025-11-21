using UnityEngine;

public class GameManager : MonoBehaviour
{
    // This is the Singleton pattern. It allows other scripts to easily access the GameManager.
    public static GameManager Instance { get; private set; }

    // This is now ONLY used to set the very first spawn point.
    public Transform playerTransform; 

    private Vector3 lastCheckpointPosition;

    private void Awake()
    {
        // --- Singleton Setup ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // When the game starts, the initial spawn point is the first checkpoint.
        // This is a good way to ensure there's always a valid spawn location.
        if (playerTransform != null)
        {
            lastCheckpointPosition = playerTransform.position;
        }
        else
        {
            Debug.LogError("Player Transform has not been assigned in the GameManager Inspector!");
        }
    }

    // Called by a Checkpoint when the player touches it.
    public void UpdateCheckpoint(Vector3 newPosition)
    {
        Debug.Log("Checkpoint updated to: " + newPosition);
        lastCheckpointPosition = newPosition;
    }

    // Called by the Player when it dies.
    public void RespawnPlayer(Player player) // <-- THE FIX IS HERE
    {
        if (player != null)
        {
            // Move the player to the last checkpoint.
            player.transform.position = lastCheckpointPosition;

            // Reset the player's state by calling their public Respawn method.
            player.Respawn();
            
            // Stop any leftover momentum from dying.
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            Debug.Log("Player has respawned.");
        }
    }
}