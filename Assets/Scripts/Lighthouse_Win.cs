using UnityEngine;
using UnityEngine.SceneManagement; // Required to change scenes

public class LighthouseWin : MonoBehaviour
{
    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu"; // Make sure this matches your scene name exactly

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            if (player != null)
            {
                // Check the boolean we added to the Player script
                if (player.hasKey == true)
                {
                    // WIN CONDITION MET
                    Debug.Log("You have the key! You win!");
                    
                    // Load the Main Menu
                    SceneManager.LoadScene(mainMenuSceneName);
                }
                else
                {
                    // Key is missing. 
                    // We do nothing here, so the "DialogueTrigger" script 
                    // on this same object can do its job and show the text.
                    Debug.Log("Door is locked.");
                }
            }
        }
    }
}