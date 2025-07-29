using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Photon.Pun;

public class NpcWalk : MonoBehaviourPun
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

    private NpcInteraction npcInteraction;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        wanderTimer = Random.Range(3f, 8f);
        agent.avoidancePriority = Random.Range(10, 99);
        agent.speed = 3f;

        npcInteraction = GetComponent<NpcInteraction>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (npcInteraction != null && npcInteraction.IsInteracting())
        {
            animator.SetFloat("Speed", 0f);
            agent.isStopped = true;
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
                                     (!agent.hasPath || agent.velocity.sqrMagnitude == 0.10f);//durma koşulunu daha erken fark et

        // Göreve ulaştıysa görevi bırak
        if (hasReachedDestination && currentTarget != null)
        {
            TaskPoint taskScript = currentTarget.GetComponent<TaskPoint>();
            taskScript?.Release(gameObject);
            currentTarget = null;
        }

        if (timer >= wanderTimer || hasReachedDestination)
        {
            float actionChoice = Random.value;

            if (actionChoice < 0.1f)
            {
                LeaveCurrentTask();  // Boşta dolaşmaya başlamadan önce görevi bırak
                isIdle = true;
                idleTimer = Random.Range(2f, 5f);
                agent.ResetPath();
                agent.isStopped = true;
                animator.SetFloat("Speed", 0f);
                return;
            }

            if (actionChoice < 0.8f && taskPoints.Length > 0)
            {
                List<Transform> shuffledPoints = new List<Transform>(taskPoints);
                for (int i = 0; i < shuffledPoints.Count; i++)
                {
                    Transform temp = shuffledPoints[i];
                    int randomIndex = Random.Range(i, shuffledPoints.Count);
                    shuffledPoints[i] = shuffledPoints[randomIndex];
                    shuffledPoints[randomIndex] = temp;
                }

                bool foundTask = false;

                foreach (Transform candidate in shuffledPoints)
                {
                    TaskPoint taskScript = candidate.GetComponent<TaskPoint>();
                    if (taskScript != null && taskScript.TryOccupy(gameObject))
                    {
                        LeaveCurrentTask(); // Önce eski görevi bırak
                        currentTarget = candidate;
                        agent.SetDestination(currentTarget.position);
                        npcInteraction?.SetTarget(currentTarget);
                        foundTask = true;
                        break;
                    }
                }

                if (!foundTask)
                {
                    LeaveCurrentTask(); // Hiç görev bulunamadıysa eski görevi bırak
                    currentTarget = null;
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    agent.SetDestination(newPos);
                    npcInteraction?.SetTarget(null);
                }
            }
            else
            {
                LeaveCurrentTask(); // Boşta dolaşmaya başlamadan önce görevi bırak
                currentTarget = null;
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                npcInteraction?.SetTarget(null);
            }

            wanderTimer = Random.Range(3f, 8f);
            agent.speed = 3f;
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

    private void LeaveCurrentTask()
    {
        if (currentTarget != null)
        {
            TaskPoint taskScript = currentTarget.GetComponent<TaskPoint>();
            taskScript?.Release(gameObject);
            currentTarget = null;
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
