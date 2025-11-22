using UnityEngine;

public class CrystalPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player touched the crystal
        if (collision.CompareTag("Player"))
        {
            // Get the Player script
            Player player = collision.GetComponent<Player>();

            if (player != null)
            {
                // 1. Give the player the key
                player.hasKey = true;

                // 2. Optional: Add a sound effect or particle here
                Debug.Log("Player picked up the Crystal!");

                // 3. Destroy this crystal object
                Destroy(gameObject);
            }
        }
    }
}