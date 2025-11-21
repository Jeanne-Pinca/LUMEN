using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is the player.
        if (other.CompareTag("Player"))
        {
            // Tell the GameManager to update the checkpoint position to this object's position.
            GameManager.Instance.UpdateCheckpoint(transform.position);

            // Optional: Give the player some feedback.
            Debug.Log("Checkpoint activated!");
            // You could play a sound, particle effect, or change sprite here.

            // Disable the collider so this checkpoint can't be triggered again.
            GetComponent<Collider2D>().enabled = false;
        }
    }
}