using StarterAssets;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public ThirdPersonController playerShooting;
    private float timeRemaining = 300f;
    private bool gameActive = true;
    private int enemiesHit = 0;
    private int totalEnemies = 20;

    void Update()
    {
        if (!gameActive) return;

        timeRemaining -= Time.deltaTime;
        uiManager.UpdateTimer(timeRemaining);

        if (timeRemaining <= 0)
        {
            EndGame(false);
        }
    }

    public void EnemyHit()
    {
        enemiesHit++;
        if (enemiesHit >= totalEnemies)
        {
            EndGame(true);
        }
    }

    void EndGame(bool win)
    {
        gameActive = false;
        playerShooting.enabled = false;
        uiManager.ShowGameOver(win ? "You Win!" : "You Lost!");
    }
}