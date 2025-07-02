using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsMenuOpened = false;
    public GameObject pauseMenu;

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
        Launcher.instance.LeaveRoom();
    }

    public void QuitGame()
    {
        Debug.Log("GameLeaved");
        Application.Quit();
    }
}
