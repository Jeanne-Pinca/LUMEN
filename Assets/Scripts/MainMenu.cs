using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame() // loads the scene named "GameScene" or whatever is assigned upon clicking the start button
    {
        SceneManager.LoadScene("GameScene"); 
    }

    public void QuitGame() // quits the application upon clicking the quit button
    {
        Application.Quit(); 
    }

}
