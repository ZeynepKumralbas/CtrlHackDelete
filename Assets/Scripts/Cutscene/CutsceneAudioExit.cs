using UnityEngine;
using UnityEngine.Playables;
using Photon.Pun;

public class CutsceneAutoExit : MonoBehaviour
{
    public PlayableDirector director;

    private bool exited = false;

    void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

    //    if (director != null)
      //      director.stopped += OnCutsceneFinished;
    }

    void Update()
    {
        if (exited) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SkipCutscene();
        }
    }

    private void OnCutsceneFinished(PlayableDirector d)
    {
        SkipCutscene();
    }

    void SkipCutscene()
    {
        if (exited) return;
        exited = true;

        Launcher.instance.returnToMenuScene = true;
        PhotonNetwork.LeaveRoom();
    }
}
