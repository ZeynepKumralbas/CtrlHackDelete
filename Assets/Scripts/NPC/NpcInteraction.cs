using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class NpcInteraction : MonoBehaviourPun
{
    public float interactionDuration = 10f;

    private NavMeshAgent agent;
    private Animator animator;
    private float interactionTimer;
    private bool isInteracting = false;
    private Transform currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;  // Sadece sahibi çalıştır

        if (isInteracting)
        {
            interactionTimer -= Time.deltaTime;
            animator.SetFloat("Speed", 0f);

            if (interactionTimer <= 0f)
            {
                isInteracting = false;
                agent.isStopped = false;

                // Tüm clientlarda animasyon parametresini güncelle
                photonView.RPC("SetInteractingState", RpcTarget.All, false);

                if (currentTarget != null)
                {
                    TaskPoint tp = currentTarget.GetComponent<TaskPoint>();
                    tp?.Release(gameObject);
                    currentTarget = null;
                }
            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
                 currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) < 1f)
        {
            isInteracting = true;
            agent.isStopped = true;

            // Tüm clientlarda animasyon parametresini güncelle
            photonView.RPC("SetInteractingState", RpcTarget.All, true);

            interactionTimer = interactionDuration;
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }

    [PunRPC]
    private void SetInteractingState(bool state)
    {
        animator.SetBool("IsInteracting", state);
    }
}
