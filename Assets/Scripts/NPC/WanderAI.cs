using UnityEngine;
using UnityEngine.AI;

public class WanderAI : MonoBehaviour
{
    public float wanderRadius = 30f;
    public float wanderTimer = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        bool hasReachedDestination = !agent.pathPending &&
                                     agent.remainingDistance <= agent.stoppingDistance &&
                                     (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);

        if (timer >= wanderTimer || hasReachedDestination)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        bool isMoving = agent.velocity.magnitude > 0.1f && !agent.pathPending;
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            Quaternion rot = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}