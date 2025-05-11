using UnityEngine;
using UnityEngine.AI;

public class GeneralEnemyBehavior : MonoBehaviour
{
    public enum EnemyType { Melee, Archer }

    [Header("General Settings")]
    public EnemyType type;
    public Transform player;
    public float chaseDistance = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Ranged Settings (For Archers)")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float arrowSpeed = 20f;

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Movement
        if (distance <= chaseDistance)
        {
            if (type == EnemyType.Archer && distance <= attackRange)
            {
                agent.SetDestination(transform.position); // Hold position
            }
            else
            {
                agent.SetDestination(player.position); // Chase
                animator.SetBool("Walking", true);
            }
        }
        else
        {
            animator.SetBool("Walking", false);
            agent.SetDestination(transform.position); // Stay still
        }

        // Attack
        if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;

            if (type == EnemyType.Melee)
                MeleeAttack();
            else
                RangedAttack();
        }
    }

    public void SetDead(bool value)
    {
        isDead = value;
        agent.isStopped = true;
    }

    private void MeleeAttack()
    {
        // Hit detection should happen through an animation event and collider
    }

    private void RangedAttack()
    {
        if (arrowPrefab != null && shootPoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            
        }
    }
}
