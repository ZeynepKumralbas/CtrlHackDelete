using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public bool IsMenuOpened = false;
    public GameObject pauseMenu;
    public GameObject gameEndPanel;
    public TMPro.TMP_Text gameEndReasonText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        bool isPaused = pauseMenu.activeSelf;
        pauseMenu.SetActive(!isPaused);

        var player = FindObjectsOfType<PhotonView>().FirstOrDefault(p => p.IsMine)?.gameObject;

        if (player != null)
        {
            var moveScript = player.GetComponent<PlayerMovement>();
            var playerInteractionScript = player.GetComponent<PlayerInteraction>();

            if (moveScript != null && playerInteractionScript != null)
            {
                moveScript.enabled = isPaused;
                playerInteractionScript.enabled = isPaused;
            }
        }

        Cursor.lockState = isPaused ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isPaused;
        //    AudioListener.pause = !isPaused;
    }

    public void LeaveGame()
    {
        Debug.Log("Game manager leave game");
        //     if (PhotonNetwork.IsMasterClient)
        //    {
        // Oyunu tüm oyuncular için kapat
        //  photonView.RPC("RPC_EndGame", RpcTarget.All);
        //   }
        //   else
        //   {
        //       // Master değilse sadece çık
        //       Launcher.instance.LeaveRoom();

        //    }
        Launcher.instance.returnToMenuScene = true;
        PhotonNetwork.LeaveRoom();

    }

    [PunRPC]
    public void RPC_ShowExitReasonAndEnd(string reason)
    {
        Debug.Log("RPC_ShowExitReasonAndEnd");
        StartCoroutine(ShowEndMessageAndExit(reason));
    }

    private IEnumerator ShowEndMessageAndExit(string reason)
    {
        Debug.Log("ShowEndMessageAndExit");
        if (gameEndPanel != null && gameEndReasonText != null)
        {
            Debug.Log("not menu");
            gameEndPanel.SetActive(true);
            gameEndReasonText.text = reason + "\nReturning to menu...";

            yield return new WaitForSeconds(3f);
        }

        Launcher.instance.returnToMenuScene = true;
        PhotonNetwork.LeaveRoom();
    }

    public void QuitGame()
    {
        Debug.Log("GameLeaved");
        Application.Quit();
    }

    [PunRPC]
    void RPC_EndGame()
    {
        Debug.Log("Game is ending for everyone...");

        Launcher.instance.returnToMenuScene = true;
        PhotonNetwork.LeaveRoom();
    }
}
