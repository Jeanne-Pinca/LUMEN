using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea] // Gives you a bigger box to type in the Inspector
    public string message = "I can't open the lighthouse yet,\nI forgot the crystal at the basement";
    
    public GameObject floatingTextPrefab; // Drag your text prefab here
    public Vector3 spawnOffset = new Vector3(0, 2.5f, 0); // Adjust height above player

    // Debounce to prevent spamming (optional)
    private float cooldown = 2f; 
    private float lastTriggerTime = -10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check cooldown so text doesn't overlap messily
            if (Time.time - lastTriggerTime > cooldown)
            {
                ShowFloatingText(other.transform.position);
                lastTriggerTime = Time.time;
            }
        }
    }

    void ShowFloatingText(Vector3 playerPosition)
    {
        if (floatingTextPrefab != null)
        {
            // Instantiate the text at the player's position + offset
            GameObject popup = Instantiate(floatingTextPrefab, playerPosition + spawnOffset, Quaternion.identity);
            
            // Set the message
            popup.GetComponent<FloatingText>().SetText(message);
        }
    }
}