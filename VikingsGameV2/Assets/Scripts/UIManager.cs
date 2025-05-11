using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private int score = 0;

    void Start()
    {
        scoreText.text = "Score: 0";
    }

    public void UpdateMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }

    public void UpdateScore()
    {
        score += 1;
        scoreText.text = $"Score: {score}";
    }

    public void UpdateTimer(float time)
    {
        timerText.text = $"Time: {Mathf.CeilToInt(time)}";
    }

    public void ShowGameOver(string message)
    {
        messageText.text = message;
        messageText.color = Color.red;
        messageText.fontSize = 40;
    }
}