using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherAudioManager : MonoBehaviour
{
    public static WatcherAudioManager Instance;

    public AudioSource watcherAudioSource;

    [SerializeField] private List<AudioClip> watcherAudioSounds;

    private AudioClip currentClip;

    private SettingsManager settingsManager;

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

        settingsManager = FindObjectOfType<SettingsManager>();
        watcherAudioSource.volume = settingsManager.settingsVolume;
    }

    public void PlayAudioClip(string audioName)
    {
        foreach (AudioClip clip in watcherAudioSounds)
        {
            if (clip.name == audioName)
            {
                currentClip = clip;
                if (clip.name.Contains("notification")) //2D  Ses
                {
                    if (view.IsMine)
                    {
                        Play2DClip(clip);
                    }
                }
                else                        //3D Ses
                {
                    view.RPC("PlayClip", RpcTarget.All, clip.name);
                }
                break;
            }
        }
    }
    [PunRPC]
    public void PlayClip(string clipName)
    {
        if(currentClip.name == clipName)
        {
            watcherAudioSource.spatialBlend = 1f;
            watcherAudioSource.PlayOneShot(currentClip);
        }
    }
    public void Play2DClip(AudioClip clip)
    {
        watcherAudioSource.spatialBlend = 0f;
        watcherAudioSource.PlayOneShot(clip);
    }
}
