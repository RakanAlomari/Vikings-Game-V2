using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public int damage = 1;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnCollisionEnter(Collision other)
    {
        
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            Destroy(rb);

        Destroy(gameObject, 2f); 
    }
}