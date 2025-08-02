using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource gameAudioSource;

    [SerializeField] private List<AudioClip> menuAudioSounds;

    private bool isBackground = false;

    /* MENÜ VE OYUN SAHNESÝ DIÞI SAHNE SESLERÝ -- 2D SES*/
    /*
    ambientSound
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

        PlayAudioClip("menuBackgroundSound");
    }
    private void Update()
    {
        if (isBackground)
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                gameAudioSource.Stop();
            }
        }
    }

    public void PlayAudioClip(string audioName)
    {
        PlayClip(audioName);
    }
    public void PlayClip(string audioName)
    {
        foreach (AudioClip clip in menuAudioSounds)
        {
            if (clip.name == audioName)
            {
                if (clip.name.Contains("Background"))
                {
                    gameAudioSource.loop = true;
                    gameAudioSource.clip = clip;
                    gameAudioSource.Play();

                    isBackground = true;
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
