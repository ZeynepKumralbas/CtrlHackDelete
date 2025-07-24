using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Photon.Pun;  // Photon namespace'i ekle

public class NpcWalk : MonoBehaviourPun  // MonoBehaviourPun olarak değiştir
{
    public float wanderRadius = 30f;
    public float wanderTimer = 5f;
    public Transform[] taskPoints;

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private bool isIdle = false;
    private float idleTimer = 0f;
    private Transform currentTarget;

    private NpcInteraction npcInteraction; // Interaction scriptine erişim

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        wanderTimer = Random.Range(3f, 8f);
        agent.avoidancePriority = Random.Range(10, 99);
        agent.speed = 1.5f;

        npcInteraction = GetComponent<NpcInteraction>(); // Interaction script cache
    }

    void Update()
    {
        if (!photonView.IsMine) return;  // Sadece sahibi NPC yürüsün ve animasyonları tetiklesin

        if (npcInteraction != null && npcInteraction.IsInteracting())
        {
            animator.SetFloat("Speed", 0f);
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

        if (timer >= wanderTimer || hasReachedDestination)
        {
            float actionChoice = Random.value;

            if (actionChoice < 0.1f)
            {
                isIdle = true;
                idleTimer = Random.Range(2f, 5f);
                agent.ResetPath();
                agent.isStopped = true;
                animator.SetFloat("Speed", 0f);
                return;
            }

            if (actionChoice < 0.8f && taskPoints.Length > 0)
            {
                // taskPoints listesini karıştır ve uygun olan ilk görevi seç
                List<Transform> shuffledPoints = new List<Transform>(taskPoints);
                for (int i = 0; i < shuffledPoints.Count; i++)
                {
                    Transform temp = shuffledPoints[i];
                    int randomIndex = Random.Range(i, shuffledPoints.Count);
                    shuffledPoints[i] = shuffledPoints[randomIndex];
                    shuffledPoints[randomIndex] = temp;
                }

                foreach (Transform candidate in shuffledPoints)
                {
                    TaskPoint taskScript = candidate.GetComponent<TaskPoint>();
                    if (taskScript != null && taskScript.TryOccupy(gameObject))
                    {
                        currentTarget = candidate;
                        agent.SetDestination(currentTarget.position);
                        npcInteraction?.SetTarget(currentTarget);
                        break;
                    }
                }
            }
            else
            {
                currentTarget = null;
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);

                npcInteraction?.SetTarget(null); // Etkileşim olmayacak
            }

            wanderTimer = Random.Range(3f, 8f);
            agent.speed = 1.5f;
            agent.isStopped = false;
            timer = 0f;
        }

        float speedRatio = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", speedRatio);

        if (speedRatio > 0.1f && !agent.pathPending)
        {
            Quaternion rot = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}

