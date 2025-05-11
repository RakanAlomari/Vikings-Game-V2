using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    private NavMeshAgent agent;
    private float detectionRange = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null) return; // Exit if player is destroyed

        // Check distance to player
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Follow player
            agent.SetDestination(player.position);
        }
        else
        {
            // Stop moving if player is out of range
            agent.SetDestination(transform.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destroy the agent if it collides with Player or Arrow
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Arrow"))
        {
            Destroy(gameObject);
        }
    }
}
