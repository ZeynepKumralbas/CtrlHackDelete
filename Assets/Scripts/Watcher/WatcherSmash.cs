using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WatcherSmash : MonoBehaviour
{
    [SerializeField] private InputActionReference interaction;

    [SerializeField] private int watcherHealth = 10;
    private Slider watcherHealthSlider;

    private Animator animator;

    public PhotonView view;
    void Start()
    {
        animator = GetComponent<Animator>();

        watcherHealthSlider = UIManager.Instance.watcherHealthSlider;
        watcherHealthSlider.maxValue = watcherHealth;
        watcherHealthSlider.value = watcherHealth;
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

                else
                {
                    if (WatcherInteraction.Instance.targetView != null)
                    {
                        view.RPC("SmashAnimation", RpcTarget.All);
                        WatcherInteraction.Instance.targetView.RPC("Die", RpcTarget.All);

                        watcherHealth--;
                        watcherHealthSlider.value = watcherHealth;
                        watcherHealthSlider.transform.Find("TxtWatcherHealth").
                            gameObject.GetComponent<TextMeshProUGUI>().text = watcherHealth.ToString() + " / " + watcherHealthSlider.maxValue.ToString();
                        

                        if(watcherHealth == 0)
                        {
                            view.RPC("Die", RpcTarget.All);
                        }
                    }
                }
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
