using UnityEngine;
using Photon.Pun;

public class PlayerAudioManager : MonoBehaviourPunCallbacks
{
    public static PlayerAudioManager Instance;

    private PhotonView view;
    private AudioSource playerAudioSource;

    public AudioClip[] playerAudioClips;

    // Loop için kontrol değişkenleri (her client'ta ayrı takip)
    private string currentLoopingClipName = "";
    private bool isLooping = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        playerAudioSource = GetComponent<AudioSource>();
        view = GetComponent<PhotonView>();
    }

    // Normal veya looping sesleri yönetmek için genel fonksiyon
    public void PlayAudioClip(string clipName)
    {
        // Eğer yürüyüş veya koşma sesi ise loop olarak çal (tüm client'larda)
        if (clipName == "walkingSound" || clipName == "runningSound")
        {
            PlayLoopingAudio(clipName);
            return;
        }
        else
        {
            // Loop sesi dışındaki ses çalınacaksa, varsa aktif loop'u tüm client'larda durdur
            if (isLooping)
                StopLoopingAudio();
        }

        AudioClip clipToPlay = FindClipByName(clipName);
        if (clipToPlay == null)
        {
            Debug.LogWarning($"Clip '{clipName}' not found in PlayerAudioClips.");
            return;
        }

        if (clipName.Contains("mission"))
        {
            // 2D Ses → sadece kendi duyacak
            if (view.IsMine)
            {
                playerAudioSource.spatialBlend = 0f; // 2D
                playerAudioSource.PlayOneShot(clipToPlay);
            }
        }
        else
        {
            // 3D Ses → herkese gönder
            view.RPC("PlayClip", RpcTarget.All, clipName);
        }
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
    // Loop eden ses çalma (tüm client'larda)
    public void PlayLoopingAudio(string clipName)
    {
        if (!view.IsMine) return;

        // Tüm oyuncularda başlatılması için RPC
        view.RPC("PlayLoopingAudioRPC", RpcTarget.All, clipName);
    }

    [PunRPC]
    private void PlayLoopingAudioRPC(string clipName)
    {
        if (currentLoopingClipName == clipName && isLooping) return;

        // Varsa önceki looping sesi durdur
        StopLocalLooping();

        AudioClip clipToPlay = FindClipByName(clipName);
        if (clipToPlay == null)
        {
            Debug.LogWarning($"[RPC] Looping Clip '{clipName}' not found.");
            return;
        }

        playerAudioSource.clip = clipToPlay;
        playerAudioSource.loop = true;
        playerAudioSource.spatialBlend = 1f;
        playerAudioSource.Play();

        isLooping = true;
        currentLoopingClipName = clipName;
    }
    // Loop eden sesi durdurma (tüm client'larda)
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

    // Sadece bu instance'ta loop sesi durdurur (RPC çağrısında kullanılır)
    private void StopLocalLooping()
    {
        if (!isLooping) return;

        playerAudioSource.Stop();
        playerAudioSource.clip = null;
        playerAudioSource.loop = false;

        isLooping = false;
        currentLoopingClipName = "";
    }

    // Yardımcı: isimle clip bulma
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

