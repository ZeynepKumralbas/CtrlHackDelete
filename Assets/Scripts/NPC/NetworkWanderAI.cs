using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class NetworkWanderAI : MonoBehaviourPun
{
    [Header("Wandering")]
    public float wanderRadius = 30f;
    public float wanderTimer = 5f;

    [Header("Task Logic")]
    public Transform[] taskPoints;          // Görev noktaları
    public float taskChance = 0.3f;         // Göreve gitme olasılığı
    public float taskDuration = 5f;         // Görev animasyonu süresi

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private bool isDoingTask = false;
    private float taskTimer = 0f;
    private bool isWalking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = wanderTimer;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Görevdeyse sadece timer çalıştır
        if (isDoingTask)
        {
            taskTimer += Time.deltaTime;
            if (taskTimer >= taskDuration)
            {
                isDoingTask = false;
                animator.SetBool("isWorking", false);
                timer = wanderTimer; // Tekrar gezinmeye başlasın
            }
            return;
        }

        timer += Time.deltaTime;

        bool hasReachedDestination = !agent.pathPending &&
                                     agent.remainingDistance <= agent.stoppingDistance &&
                                     (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);

        if (timer >= wanderTimer || hasReachedDestination)
        {
            timer = 0f;

            // Görev mi yapacak yoksa gezinecek mi?
            if (taskPoints.Length > 0 && Random.value < taskChance)
            {
                Transform taskPoint = taskPoints[Random.Range(0, taskPoints.Length)];
                agent.SetDestination(taskPoint.position);
                isDoingTask = true;
                taskTimer = 0f;
                animator.SetBool("isWorking", true);
                animator.SetBool("isWalking", false);
            }
            else
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
            }
        }

        // Yürüme animasyonu
        isWalking = agent.velocity.magnitude > 0.1f && !agent.pathPending;
        animator.SetBool("isWalking", isWalking);

        // NPC'nin yüzünü hareket yönüne döndür
        if (isWalking)
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
