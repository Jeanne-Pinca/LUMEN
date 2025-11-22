using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame() // loads the scene named "GameScene" or whatever is assigned upon clicking the start button
    {
        SceneManager.LoadScene("First Level"); 
    }

    public void QuitGame() // quits the application upon clicking the quit button
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // stops play mode in the editor
        #endif
        Application.Quit(); // quits the application
    }

}
