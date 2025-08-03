using UnityEngine;
using UnityEngine.Playables;
using Photon.Pun;

public class CutsceneAutoExit : MonoBehaviour
{
    private bool exited = false;

    void Update()
    {
        if (exited) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            exited = true;
            Launcher.instance.returnToMenuScene = true;
            PhotonNetwork.LeaveRoom();
        }
    }
}
