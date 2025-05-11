using UnityEngine;

public class HitboxAttack : MonoBehaviour
{
    public int damage = 1; 

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }
 

}