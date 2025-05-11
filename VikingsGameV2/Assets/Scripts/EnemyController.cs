using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private GameObject player;
    private bool isHit = false;
    private float fleeDistance = 10f;
    private float detectionDistance = 4f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        Wander();
    }

    void Update()
    {
        if (isHit) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionDistance)
        {
            Flee();
        }
        else if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            Wander();
        }
    }

    void Wander()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * 10f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void Flee()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        Vector3 fleeDirection = (transform.position - player.transform.position).normalized;
        Vector3 fleePoint = transform.position + fleeDirection * fleeDistance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePoint, out hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    public void Hit()
    {
        if (!isHit)
        {
            isHit = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
            agent.isStopped = true;
        }
    }

    public bool IsHit()
    {
        return isHit;
    }
}