using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherAudioManager : MonoBehaviour
{
    public static WatcherAudioManager Instance;

    public AudioSource watcherAudioSource;

    [SerializeField] private List<AudioClip> watcherAudioSounds;

    public PhotonView view;

    /* WATCHER SESLERÝ -- 3D SES / 2D SES*/
    /*
    walkingSound
    runningSound --> walkingSound AudioClip üzerinden ayar çekilip runningSound için kullanýlabilir

    notificationSound
    smashSound

    skill_FreezeEffectSound
    skill_CloseSightEffectSound

    watcherDeathSound
    */

    void Start()
    {
        Instance = this;

        watcherAudioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(string audioName)
    {
        foreach (AudioClip clip in watcherAudioSounds)
        {
            if (clip.name == audioName)
            {
                if (clip.name.Contains("notification")) //2D  Ses
                {
                    if (view.IsMine)
                    {
                        Play2DClip(clip);
                    }
                }
                else                        //3D Ses
                {
                    view.RPC("PlayClip", RpcTarget.All, clip);
                }
                break;
            }
        }
    }
    [PunRPC]
    public void PlayClip(AudioClip clip)
    {
        watcherAudioSource.spatialBlend = 1f;
        watcherAudioSource.PlayOneShot(clip);
    }
    public void Play2DClip(AudioClip clip)
    {
        watcherAudioSource.spatialBlend = 0f;
        watcherAudioSource.PlayOneShot(clip);
    }
}
