using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider sfxSlider;
    public Slider musicSlider;

    private float defaultSFXVolume = 1f;
    private float defaultMusicVolume = 1f;

    public float settingsVolume;

    void Start()
    {
        // PlayerPrefs'ten kaydedilen ayarlarý yükle
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);

        // Ses seviyelerini uygula (örnek: burada bir audio mixer varsa baðlantý yapýlabilir)
        SetSFXVolume(sfxSlider.value);
        SetMusicVolume(musicSlider.value);

        // Deðiþiklikleri dinle
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();

        settingsVolume = volume;

        // Burada kendi SFX ses kanalýna uygulaman gerekir
        Debug.Log("SFX volume: " + volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();

        AudioManager.Instance.gameAudioSource.volume = volume;

        // Burada müzik ses kanalýna uygulaman gerekir
        Debug.Log("Music volume: " + volume);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
