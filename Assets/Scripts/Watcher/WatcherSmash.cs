using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WatcherSmash : MonoBehaviour
{
    [SerializeField] private InputActionReference interaction;

    private Animator animator;

    public PhotonView view;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!view.IsMine) return;

        if (WatcherInteraction.Instance.inHitbox)
        {
            if (interaction.action.IsPressed())
            {
                if (WatcherInteraction.Instance.isPlayer)
                {
                    view.RPC("SmashAnimation", RpcTarget.All);
                    WatcherInteraction.Instance.targetView.RPC("Die", RpcTarget.All);
                }

                /*    else
                    {
                        
                    }
                */   
            
            }
        }
    }

    [PunRPC]
    public void SmashAnimation()
    {
        animator.SetTrigger("isSmashing");
        WatcherInteraction.Instance.inHitbox = false;
    }

}
