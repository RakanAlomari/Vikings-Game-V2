using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeScreen : MonoBehaviour
{
    public TextMeshProUGUI welcomeText;
    private float displayTime = 5f;

    void Start()
    {
        welcomeText.text = "Welcome to the Game!\n" +
                           "Name: Rakan Alomari\n" +
                           "Student ID: 202042180\n" +
                           "Theme: Vikings";
        Invoke("LoadGameScene", displayTime);
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("Village");
    }
}