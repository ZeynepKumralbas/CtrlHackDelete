using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Animator animator;
    private bool isDead = false;

    public PhotonView view;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    [PunRPC]
    public void Die()
    {
        if(isDead) return;
        isDead = true;

        animator.SetTrigger("getHit");

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerInteraction>().enabled = false;

    }
}
