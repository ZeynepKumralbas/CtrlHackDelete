using UnityEngine;
using UnityEngine.AI;

public class NetworkWanderAI : MonoBehaviour
{
    public float wanderRadius = 30f;
    public float wanderTimer = 5f;
    public float interactionDuration = 10f;
    public Transform[] taskPoints;

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private bool isInteracting = false;
    private float interactionTimer;
    private Transform currentTarget;

    private bool isIdle = false;       // Sabit durma kontrolü
    private float idleTimer = 0f;      // Sabit durma süresi

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        wanderTimer = Random.Range(3f, 8f);

        agent.avoidancePriority = Random.Range(10, 99);
    }

    void Update()
    {
        if (isInteracting)
        {
            interactionTimer -= Time.deltaTime;
            animator.SetFloat("Speed", 0f);

            if (interactionTimer <= 0f)
            {
                isInteracting = false;
                agent.isStopped = false;
                animator.SetBool("IsInteracting", false);
                SelectNewDestination();
            }
            return;
        }

        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            animator.SetFloat("Speed", 0f);
            agent.isStopped = true;

            if (idleTimer <= 0f)
            {
                isIdle = false;
                agent.isStopped = false;
                timer = 0f;
            }
            return;
        }

        timer += Time.deltaTime;

        bool hasReachedDestination = !agent.pathPending &&
                                     agent.remainingDistance <= agent.stoppingDistance &&
                                     (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);

        if ((timer >= wanderTimer || hasReachedDestination) && !isInteracting && !isIdle)
        {
            float actionChoice = Random.value;

            if (actionChoice < 0.4f)
            {
                // %40 sabit durma (idle)
                isIdle = true;
                idleTimer = Random.Range(2f, 5f);
                agent.ResetPath();
                agent.isStopped = true;
                animator.SetFloat("Speed", 0f);
                return;
            }

            if (actionChoice < 0.4f + 0.42f && taskPoints.Length > 0)
            {
                // %42 görev noktasına git
                currentTarget = taskPoints[Random.Range(0, taskPoints.Length)];
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                // %18 rastgele dolaşma
                currentTarget = null;
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
            }

            // Hareket türü: %30 koşu (5–10 saniye), %70 yürüme
            if (Random.value < 0.3f)
            {
                agent.speed = 3.5f;
                wanderTimer = Random.Range(5f, 10f);
            }
            else
            {
                agent.speed = 1.5f;
                wanderTimer = Random.Range(3f, 8f);
            }

            agent.isStopped = false;
            timer = 0f;
        }

        // Animasyon ve dönüş
        float speedRatio = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", speedRatio);

        if (speedRatio > 0.1f && !agent.pathPending)
        {
            Quaternion rot = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }

        // Görev noktasına ulaştıysa interaction başlat
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
            currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) < 1f)
        {
            isInteracting = true;
            agent.isStopped = true;
            animator.SetBool("IsInteracting", true);
            interactionTimer = interactionDuration;
            currentTarget = null;
        }
    }

    private void SelectNewDestination()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);

        // Hareket türü: %30 koşu, %70 yürüme
        if (Random.value < 0.3f)
        {
            agent.speed = 3.5f;
            wanderTimer = Random.Range(5f, 10f);
        }
        else
        {
            agent.speed = 1.5f;
            wanderTimer = Random.Range(3f, 8f);
        }

        agent.isStopped = false;
        timer = 0f;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}
