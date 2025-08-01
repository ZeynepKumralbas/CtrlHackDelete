using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public static PlayerAudioManager Instance;

    public AudioSource playerAudioSource;

    [SerializeField] private List<AudioClip> playerAudioSounds;

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
    }

    public void PlayAudioClip(string audioName)
    {
        foreach (AudioClip clip in playerAudioSounds)
        {
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
                    view.RPC("PlayClip", RpcTarget.All, clip);
                }
                break;
            }
        }
    }

    [PunRPC]
    public void PlayClip(AudioClip clip)
    {
        playerAudioSource.spatialBlend = 1f;
        playerAudioSource.PlayOneShot(clip);
    }
    public void Play2DClip(AudioClip clip)
    {
        playerAudioSource.spatialBlend = 0f;
        playerAudioSource.PlayOneShot(clip);
    }
}
