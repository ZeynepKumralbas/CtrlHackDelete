using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;

public class NpcDeath : MonoBehaviourPun
{
    private Animator animator;
    private NavMeshAgent agent;
    private bool isDead = false;

    [SerializeField] private float fallbackDestroyDelay = 5f; // Animasyon bulunamazsa

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    [PunRPC]
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Die");
        agent.isStopped = true;
        agent.ResetPath();

        var walk = GetComponent<NpcWalk>();
        if (walk != null) walk.enabled = false;

        var interact = GetComponent<NpcInteraction>();
        if (interact != null) interact.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // ⏳ Animasyon süresini al, yoksa yedek süre kullan
        float dieAnimLength = GetAnimationClipLength("Die");
        StartCoroutine(DestroyAfterDelay(dieAnimLength));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private float GetAnimationClipLength(string clipName)
    {
        if (animator.runtimeAnimatorController == null) return fallbackDestroyDelay;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        return fallbackDestroyDelay;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
