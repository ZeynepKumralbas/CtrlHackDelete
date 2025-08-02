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

    private int playerCount;
    void Start()
    {
        animator = GetComponent<Animator>();

        watcherHealthSlider = UIManager.Instance.watcherHealthSlider;
        watcherHealthSlider.maxValue = watcherHealth;
        watcherHealthSlider.value = watcherHealth;

        playerCount = PhotonNetwork.CurrentRoom.PlayerCount - 1;
    }

    void Update()
    {
        if (!view.IsMine) return;

        if (WatcherInteraction.Instance.inHitbox)
        {
            if (interaction.action.IsPressed()) //Player'a vurma
            {
                if (WatcherInteraction.Instance.isPlayer)
                {
                    GameObject targetPlayer = WatcherInteraction.Instance.targetView.gameObject;
                    PlayerStateManager stateManager = targetPlayer.GetComponent<PlayerStateManager>();

                    if (stateManager != null && stateManager.currentState != PlayerState.Ghost)
                    {
                        view.RPC("SmashAnimation", RpcTarget.All);
                        WatcherInteraction.Instance.targetView.RPC("Die", RpcTarget.All);


                        /* OYUN SONU SENARYOSU ---> TUM PLAYER'LAR OLURSE*/
                        playerCount--;

                        if (playerCount == 0)
                        {
                            if (!GameEndManager.Instance.gameEnded)
                            {
                                GameEndManager.Instance.photonView.RPC("RPC_EndGame", RpcTarget.All, "WatcherWin");
                            }
                        }
                    }
                }

                else //NPC'ye vurma
                {
                    if (WatcherInteraction.Instance.targetView != null)
                    {
                        view.RPC("SmashAnimation", RpcTarget.All);
                        WatcherInteraction.Instance.targetView.RPC("Die", RpcTarget.All);
                        WatcherAudioManager.Instance.PlayAudioClip("smashSound");

                        watcherHealth--;
                        watcherHealthSlider.value = watcherHealth;
                        watcherHealthSlider.transform.Find("TxtWatcherHealth").
                            gameObject.GetComponent<TextMeshProUGUI>().text = watcherHealth.ToString() + " / " + watcherHealthSlider.maxValue.ToString();


                        if (watcherHealth == 0)
                        {
                            view.RPC("Die", RpcTarget.All);

                            Invoke(nameof(WatcherSceneTransition), 1.0f);
                        }
                    }
                }
            }
        }
    }
    public void WatcherSceneTransition()
    {
        /* OYUN SONU SENARYOSU ---> WATCHER OLURSE*/

        if (!GameEndManager.Instance.gameEnded)
        {
            GameEndManager.Instance.photonView.RPC("RPC_EndGame", RpcTarget.All, "WatcherLose");
        }
    }

    [PunRPC]
    public void SmashAnimation()
    {
        animator.SetTrigger("isSmashing");
        WatcherInteraction.Instance.inHitbox = false;
    }

}
