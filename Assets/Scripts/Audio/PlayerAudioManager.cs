using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public static PlayerAudioManager Instance;

    public AudioSource playerAudioSource;

    [SerializeField] private List<AudioClip> playerAudioSounds;

    private AudioClip currentClip;

    private SettingsManager settingsManager;

    public PhotonView view;

    /* PLAYER SESLERÝ -- 3D SES / 2D SES*/
    /*
    walkingSound
    runningSound --> walkingSound AudioClip üzerinden ayar çekilip runningSound için kullanýlabilir

    missionMakingSound
    missionCompletedSound

    skill_FreezeActivateSound
    skill_CloseSightActivateSound
    skill_ChangecolorActivateSound

    playerDeathSound
    */

    void Start()
    {
        Instance = this;

        playerAudioSource = GetComponent<AudioSource>();

        /*settingsManager = FindObjectOfType<SettingsManager>();
        playerAudioSource.volume = settingsManager.settingsVolume;*/
    }

    public void PlayAudioClip(string audioName)
    {
        foreach (AudioClip clip in playerAudioSounds)
        {
            currentClip = clip;
            if (clip.name == audioName)
            {
                if (clip.name.Contains("skill")) //2D  Ses
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
            playerAudioSource.spatialBlend = 1f;
            playerAudioSource.PlayOneShot(currentClip);
        }
    }
    public void Play2DClip(AudioClip clip)
    {
        playerAudioSource.spatialBlend = 0f;
        playerAudioSource.PlayOneShot(clip);
    }
}
