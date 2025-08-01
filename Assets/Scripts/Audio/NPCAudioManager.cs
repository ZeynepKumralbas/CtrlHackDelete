using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAudioManager : MonoBehaviour
{
    public static NPCAudioManager Instance;

    public AudioSource npcAudioSource;

    [SerializeField] private List<AudioClip> npcAudioSounds;

    private SettingsManager settingsManager;

    public PhotonView view;

    /* NPC SESLERÝ -- 3D SES*/
    /*
    walkingSound
    runningSound --> walkingSound AudioClip üzerinden ayar çekilip runningSound için kullanýlabilir

    missionMakingSound
    missionCompletedSound

    npcDeathSound
    */

    void Start()
    {
        Instance = this;

        npcAudioSource = GetComponent<AudioSource>();

        /*settingsManager = FindObjectOfType<SettingsManager>();
        npcAudioSource.volume = settingsManager.settingsVolume;*/

        if (!view.IsMine) return;
    }

    public void PlayAudioClip(string audioName)
    {
        view.RPC("PlayClip", RpcTarget.All, audioName);
    }
    [PunRPC]
    public void PlayClip(string audioName)
    {
        foreach (AudioClip clip in npcAudioSounds)
        {
            if (clip.name == audioName)
            {
                npcAudioSource.PlayOneShot(clip);

                break;
            }
        }
    }
}
