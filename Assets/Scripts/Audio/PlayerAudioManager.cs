using UnityEngine;
using Photon.Pun;

public class PlayerAudioManager : MonoBehaviourPunCallbacks
{
    public static PlayerAudioManager Instance;

    private PhotonView view;
    private AudioSource playerAudioSource;

    public AudioClip[] playerAudioClips;

    private string currentLoopingClipName = "";
    private bool isLooping = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        view = GetComponent<PhotonView>();
        playerAudioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(string clipName)
    {
        // Loop yapılacak sesler
        if (clipName == "walkingSound" || clipName == "runningSound")
        {
            PlayLoopingAudio(clipName);
            return;
        }
        else
        {
            if (isLooping)
                StopLoopingAudio();
        }

        // Mission sesi → sadece kendi duyacak (2D)
        if (clipName == "missionMakingSound" || clipName == "missionCompletedSound")
        {
            if (view.IsMine)
            {
                AudioClip clip = FindClipByName(clipName);
                if (clip != null)
                {
                    playerAudioSource.spatialBlend = 0f; // 2D
                    playerAudioSource.PlayOneShot(clip);
                }
            }
            return;
        }

        // Diğer tüm sesler (skilller, ölüm) → herkes duyar (3D)
        view.RPC("PlayClip", RpcTarget.All, clipName);
    }

    [PunRPC]
    private void PlayClip(string clipName)
    {
        AudioClip clipToPlay = FindClipByName(clipName);
        if (clipToPlay == null)
        {
            Debug.LogWarning($"[RPC] Clip '{clipName}' not found.");
            return;
        }

        playerAudioSource.spatialBlend = 1f; // 3D
        playerAudioSource.PlayOneShot(clipToPlay);
    }

    public void PlayLoopingAudio(string clipName)
    {
        if (!view.IsMine) return;

        view.RPC("PlayLoopingAudioRPC", RpcTarget.All, clipName);
    }

    [PunRPC]
    private void PlayLoopingAudioRPC(string clipName)
    {
        if (currentLoopingClipName == clipName && isLooping) return;

        StopLocalLooping();

        AudioClip clipToPlay = FindClipByName(clipName);
        if (clipToPlay == null)
        {
            Debug.LogWarning($"[RPC] Looping Clip '{clipName}' not found.");
            return;
        }

        playerAudioSource.clip = clipToPlay;
        playerAudioSource.loop = true;
        playerAudioSource.spatialBlend = 1f; // 3D
        playerAudioSource.Play();

        isLooping = true;
        currentLoopingClipName = clipName;
    }

    public void StopLoopingAudio()
    {
        if (!view.IsMine) return;

        view.RPC("StopLoopingAudioRPC", RpcTarget.All);
    }

    [PunRPC]
    private void StopLoopingAudioRPC()
    {
        StopLocalLooping();
    }

    private void StopLocalLooping()
    {
        if (!isLooping) return;

        playerAudioSource.Stop();
        playerAudioSource.clip = null;
        playerAudioSource.loop = false;

        isLooping = false;
        currentLoopingClipName = "";
    }

    private AudioClip FindClipByName(string clipName)
    {
        foreach (AudioClip clip in playerAudioClips)
        {
            if (clip != null && clip.name == clipName)
                return clip;
        }
        return null;
    }
}
