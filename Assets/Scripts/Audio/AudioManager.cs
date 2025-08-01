using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource gameAudioSource;

    [SerializeField] private List<AudioClip> menuAudioSounds;

    public PhotonView view;

    /* MENÜ VE OYUN SAHNESÝ DIÞI SAHNE SESLERÝ -- 2D SES*/
    /*
    menuClickSound
    menuBackgroundSound
    controlsSound
    winSound
    loseSound
    */

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameAudioSource = GetComponent<AudioSource>();

        if (!view.IsMine) return;
    }

    public void PlayAudioClip(string audioName)
    {
        PlayClip(audioName);
    }
    public void PlayClip(string audioName)
    {
        if (!view.IsMine) return;

        foreach (AudioClip clip in menuAudioSounds)
        {
            if (clip.name == audioName)
            {
                if (clip.name.Contains("Background"))
                {
                    gameAudioSource.loop = true;
                    gameAudioSource.clip = clip;
                    gameAudioSource.Play();
                }
                else
                {
                    gameAudioSource.loop = false;
                    gameAudioSource.PlayOneShot(clip);
                }
                break;
            }
        }
    }


}
