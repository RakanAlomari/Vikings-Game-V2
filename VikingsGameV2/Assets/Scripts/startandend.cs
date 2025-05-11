using UnityEngine;
using UnityEngine.SceneManagement;

public class startandend : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("HubArea"); // Replace "GameScene" with your game scene name
    }

    // Function to exit the game
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the editor
        #else
            Application.Quit(); // Quits the application in a build
        #endif
    }
}
