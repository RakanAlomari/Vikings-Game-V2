using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth instance;

    [Header("Health Settings")]
    public int maxHealth = 10; 
    private int currentHealth;

    private Animator animator;
    private bool isDead = false;

    [Header("UI Settings")]
    public Slider healthSlider; 

    void Start() 
    {
        instance = this;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int amount) 
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        isDead = true;
        GetComponent<GeneralEnemyBehavior>()?.SetDead(true);
        animator.SetBool("Dead", true); 
        Destroy(gameObject, 5f); 
    }
    
    private void UpdateHealthBar() 
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}