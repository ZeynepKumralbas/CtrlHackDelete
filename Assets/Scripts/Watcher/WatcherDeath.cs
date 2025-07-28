using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherDeath : MonoBehaviour
{
    private Animator animator;
    private bool isDead = false;

    public float deathDelay = 1;

    public PhotonView view;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    [PunRPC]
    public void Die()
    {
        Invoke(nameof(WatcherDie), deathDelay);
    }
    public void WatcherDie()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("getHit");

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<WatcherInteraction>().enabled = false;
        GetComponent<WatcherNotification>().enabled = false;
        GetComponent<WatcherEffectedFromPlayerSkills>().enabled = false;
        GetComponent<WatcherSmash>().enabled = false;
    }
}
